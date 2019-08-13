using System.Collections.Generic;
using DreamLib.Data.Models;
using System.Linq;
using System;

namespace DreamLib.Data.Sql
{
    public sealed class SQLiteDatalayer : IDatalayer
    {
        private readonly DreamLogDbContext dbContext;

        public SQLiteDatalayer(string filePath)
        {
            this.dbContext = new DreamLogDbContext($"Data Source={filePath};");
            this.dbContext.Database.EnsureCreated();
        }

        /// <summary>
        /// Returns all <see cref="DreamLogCollection"/>-entities from the database
        /// </summary>
        public IEnumerable<DreamLogCollection> DreamLogCollections { get => this.dbContext.DreamLogs.AsEnumerable(); }
        /// <summary>
        /// Returns all <see cref="DreamCategory"/>-entities from the database
        /// </summary>
        public IEnumerable<DreamCategory> DreamCategories { get => this.dbContext.Categories.AsEnumerable(); }
        /// <summary>
        /// Returns all <see cref="DreamLogEntry"/>-entities from the database
        /// </summary>
        public IEnumerable<DreamLogEntry> DreamLogEntries { get { return this.dbContext.Entries.AsEnumerable(); } }

        /// <summary>
        /// Adds a new <see cref="DreamCategory"/> to the database
        /// </summary>
        public bool AddDreamCategory(DreamCategory category)
        {
            lock (this.dbContext)
            {
                this.SaveChanges();
                this.dbContext.Categories.Add(category);
                return this.SaveChanges() == 1;
            }
        }

        /// <summary>
        /// Adds a new <see cref="DreamLogCollection"/> to the database
        /// </summary>
        public bool AddDreamLog(DreamLogCollection log)
        {
            lock (this.dbContext)
            {
                this.SaveChanges();
                this.dbContext.DreamLogs.Add(log);
                return this.SaveChanges() == 1;
            }
        }

        /// <summary>
        /// Adds a new <see cref="DreamLogEntry"/> to the database
        /// </summary>
        public bool AddLogEntry(DreamLogEntry entry)
        {
            lock (this.dbContext)
            {
                this.SaveChanges();
                this.dbContext.Entries.Add(entry);
                return this.SaveChanges() == 1;
            }
        }

        /// <summary>
        /// Returns the <see cref="DreamCategory"/> with the specified <see cref="DreamCategory.CategoryId"/> from the database
        /// </summary>
        public DreamCategory GetDreamCategory(long categoryId)
        {
            return this.dbContext.Categories.SingleOrDefault(_category => _category.CategoryId == categoryId);
        }

        /// <summary>
        /// Returns all <see cref="DreamLogEntry"/>-entities where <see cref="DreamLogEntry.Date"/> is between the specified
        /// <see cref="DateTime"/>-range and that are associated with the specified <see cref="DreamCategory.CategoryId"/>
        /// from the database
        /// </summary>
        public IEnumerable<DreamLogEntry> GetLogEntries(DateTime rangeStart, DateTime rangeEnd, long? categoryId)
        {
            return this.dbContext.Entries.Where(_entry => _entry.Date >= rangeStart && _entry.Date <= rangeEnd && _entry.FK_CategoryId == categoryId);
        }

        /// <summary>
        /// Returns all <see cref="DreamLogEntry"/>-entities where <see cref="DreamLogEntry.Date"/> is between the specified 
        /// <see cref="DateTime"/>-range from the database
        /// </summary>
        public IEnumerable<DreamLogEntry> GetLogEntries(DateTime rangeStart, DateTime rangeEnd)
        {
            return this.dbContext.Entries.Where(_entry => _entry.Date >= rangeStart && _entry.Date <= rangeEnd);
        }

        /// <summary>
        /// Returns all <see cref="DreamLogEntry"/>-entities that are associated with the specified 
        /// <see cref="DreamCategory.CategoryId"/> from the database
        /// </summary>
        public IEnumerable<DreamLogEntry> GetLogEntries(long? categoryId)
        {
            return this.dbContext.Entries.Where(_entry => _entry.FK_CategoryId == categoryId);
        }

        /// <summary>
        /// Returns the <see cref="DreamLogCollection"/> with the specified <see cref="DreamLogCollection.LogId"/> from the database
        /// </summary>
        public DreamLogCollection GetDreamLog(long logId)
        {
            return this.dbContext.DreamLogs.SingleOrDefault(_log => _log.LogId == logId);
        }

        /// <summary>
        /// Returns the <see cref="DreamLogEntry"/> with the specified <see cref="DreamLogEntry.EntryId"/> from the database
        /// </summary>
        public DreamLogEntry GetLogEntry(long entryId)
        {
            return this.dbContext.Entries.SingleOrDefault(_entry => _entry.EntryId == entryId);
        }

        /// <summary>
        /// Removes the <see cref="DreamCategory"/> with the specified <see cref="DreamCategory.CategoryId"/> from the database
        /// </summary>
        public bool RemoveDreamCategory(long categoryId)
        {
            DreamCategory category = this.GetDreamCategory(categoryId);
            if (!(category is null))
            {
                lock (this.dbContext)
                {
                    this.SaveChanges();
                    this.dbContext.Categories.Remove(category);
                    return this.SaveChanges() == 1;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the <see cref="DreamLogCollection"/> with the specified <see cref="DreamLogCollection.LogId"/> from the database
        /// </summary>
        public bool RemoveDreamLog(long logId)
        {
            DreamLogCollection log = this.GetDreamLog(logId);
            if (!(log is null))
            {
                lock (this.dbContext)
                {
                    this.SaveChanges();
                    this.dbContext.DreamLogs.Remove(log);
                    return this.SaveChanges() == 1;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the <see cref="DreamLogEntry"/> with the specified <see cref="DreamLogEntry.EntryId"/> from the database
        /// </summary>
        public bool RemoveLogEntry(long entryId)
        {
            DreamLogEntry entry = this.GetLogEntry(entryId);
            if (!(entry is null))
            {
                lock (this.dbContext)
                {
                    this.SaveChanges();
                    this.dbContext.Entries.Remove(entry);
                    return this.SaveChanges() == 1;
                }
            }
            return false;
        }

        /// <summary>
        /// Saves all pending changes to the database
        /// </summary>
        public int SaveChanges()
        {
            return this.dbContext.SaveChanges();
        }
    }
}
