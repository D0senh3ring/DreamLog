using System.Security.Cryptography;
using System.Collections.Generic;
using DreamLib.Data.Models;
using System.Linq;
using System.Text;
using System;

namespace DreamLib.Data.Sql
{
    public class DreamLogDatalayer : IDatalayer
    {
        private readonly Dictionary<long, string> categoryHashes = new Dictionary<long, string>();
        private readonly Dictionary<long, string> entryHashes = new Dictionary<long, string>();
        private readonly List<DreamCategory> categories = new List<DreamCategory>();
        private readonly List<DreamLogEntry> logEntries = new List<DreamLogEntry>();
        private readonly HashAlgorithm hashAlgorithm = new SHA256Managed();
        private readonly DreamLogCollection currentCollection;
        private readonly SQLiteDbConnection connection;

        public DreamLogDatalayer(string filePath)
        {
            this.connection = new SQLiteDbConnection(filePath);
            this.connection.Open();

            this.currentCollection = this.connection.GetEntities<DreamLogCollection>().FirstOrDefault();
            if (this.currentCollection is null)
            {
                this.currentCollection = new DreamLogCollection() { Name = "Default" };
                this.connection.AddEntity(this.currentCollection);
            }

            this.UpdateEntitiyLists();
            this.UpdateEntityBindings();
            this.UpdateChangeHashes();
        }

        ~DreamLogDatalayer()
        {
            this.hashAlgorithm.Dispose();
            this.connection.Close();
        }

        public IEnumerable<DreamLogCollection> DreamLogCollections => this.connection.GetEntities<DreamLogCollection>().ToArray();
        public IEnumerable<DreamCategory> DreamCategories => this.categories;
        public IEnumerable<DreamLogEntry> DreamLogEntries => this.logEntries;

        public bool AddDreamCategory(DreamCategory category)
        {
            if (this.connection.AddEntity(category))
            {
                this.categoryHashes.Add(category.CategoryId, this.GetDreamCategoryHash(category));
                this.categories.Add(category);
                return true;
            }
            return false;
        }

        public bool AddDreamLog(DreamLogCollection log)
        {
            return this.connection.AddEntity(log);
        }

        public bool AddLogEntry(DreamLogEntry entry)
        {
            if (this.connection.AddEntity(entry))
            {
                this.entryHashes.Add(entry.EntryId, this.GetLogEntryHash(entry));
                this.logEntries.Add(entry);
                return true;
            }
            return false;
        }

        public DreamCategory GetDreamCategory(long categoryId)
        {
            return this.categories.FirstOrDefault(_category => _category.CategoryId == categoryId);
        }

        public DreamLogCollection GetDreamLog(long logId)
        {
            return this.currentCollection;
        }

        public IEnumerable<DreamLogEntry> GetLogEntries(DateTime rangeStart, DateTime rangeEnd, long? categoryId)
        {
            return this.logEntries.Where(_entry => _entry.Date >= rangeStart && _entry.Date <= rangeEnd && (!categoryId.HasValue || _entry.FK_CategoryId == categoryId));
        }

        public IEnumerable<DreamLogEntry> GetLogEntries(DateTime rangeStart, DateTime rangeEnd)
        {
            return this.GetLogEntries(rangeStart, rangeEnd, null);
        }

        public IEnumerable<DreamLogEntry> GetLogEntries(long? categoryId)
        {
            return this.logEntries.Where(_entry => !categoryId.HasValue || _entry.FK_CategoryId == categoryId);
        }

        public DreamLogEntry GetLogEntry(long entryId)
        {
            return this.logEntries.FirstOrDefault(_entry => _entry.EntryId == entryId);
        }

        public bool RemoveDreamCategory(long categoryId)
        {
            DreamCategory category = this.GetDreamCategory(categoryId);
            if (!(category is null) && category.DreamLogEntries.Count == 0)
            {
                this.categories.Remove(category);
                this.categoryHashes.Remove(category.CategoryId);
                return this.connection.DeleteEntity<DreamCategory>(categoryId);
            }
            return false;
        }

        public bool RemoveDreamLog(long logId)
        {
            return false;
        }

        public bool RemoveLogEntry(long entryId)
        {
            DreamLogEntry entry = this.GetLogEntry(entryId);
            if (!(entry is null))
            {
                if (!(entry.Category is null))
                {
                    entry.Category.DreamLogEntries.Remove(entry);
                }
                this.logEntries.Remove(entry);
                this.entryHashes.Remove(entry.EntryId);
                return this.connection.DeleteEntity<DreamLogEntry>(entryId);
            }
            return false;
        }

        public int SaveChanges()
        {
            int changes = this.connection.UpdateAll(this.GetChangedCategories()) + this.connection.UpdateAll(this.GetChangedEntries());

            this.UpdateEntityBindings();
            this.UpdateChangeHashes();

            return changes;
        }

        private void UpdateEntitiyLists()
        {
            this.categories.Clear();
            this.categories.AddRange(this.connection.GetEntities<DreamCategory>());

            this.logEntries.Clear();
            this.logEntries.AddRange(this.connection.GetEntities<DreamLogEntry>());
        }

        private void UpdateEntityBindings()
        {
            foreach (DreamLogEntry entry in this.logEntries)
            {
                entry.Log = this.currentCollection;
                entry.Log.LogEntries.Add(entry);
                if (entry.FK_CategoryId.HasValue)
                {
                    entry.Category = this.categories.FirstOrDefault(_category => _category.CategoryId == entry.FK_CategoryId);
                    entry.Category.DreamLogEntries.Add(entry);
                }
            }
        }

        private void UpdateChangeHashes()
        {
            foreach (DreamLogEntry entry in this.logEntries)
            {
                string hash = this.GetLogEntryHash(entry);
                if (this.entryHashes.ContainsKey(entry.EntryId))
                    this.entryHashes[entry.EntryId] = hash;
                else
                    this.entryHashes.Add(entry.EntryId, hash);
            }

            foreach (DreamCategory category in this.categories)
            {
                string hash = this.GetDreamCategoryHash(category);
                if (this.categoryHashes.ContainsKey(category.CategoryId))
                    this.categoryHashes[category.CategoryId] = hash;
                else
                    this.categoryHashes.Add(category.CategoryId, hash);
            }
        }

        private IEnumerable<DreamCategory> GetChangedCategories()
        {
            return this.categories.Where(_category => !this.categoryHashes.ContainsKey(_category.CategoryId) || !this.categoryHashes[_category.CategoryId].Equals(this.GetDreamCategoryHash(_category))).ToArray();
        }

        private IEnumerable<DreamLogEntry> GetChangedEntries()
        {
            return this.logEntries.Where(_entry => !this.entryHashes.ContainsKey(_entry.EntryId) || !this.entryHashes[_entry.EntryId].Equals(this.GetLogEntryHash(_entry))).ToArray();
        }

        private string GetLogEntryHash(DreamLogEntry entry)
        {
            byte[] bytes = Encoding.UTF8.GetBytes($"{entry.EntryId}|{entry.FK_LogId}|{entry.FK_CategoryId}|{entry.Date}|{entry.Title}|{entry.Content}|{entry.CreatedAt}");
            return Convert.ToBase64String(this.hashAlgorithm.ComputeHash(bytes));
        }

        private string GetDreamCategoryHash(DreamCategory category)
        {
            byte[] bytes = Encoding.UTF8.GetBytes($"{category.CategoryId}|{category.Name}|{category.ColorString}|{String.Join(",", category.DreamLogEntries.Select(_entry => _entry.EntryId))}");
            return Convert.ToBase64String(this.hashAlgorithm.ComputeHash(bytes));
        }
    }
}
