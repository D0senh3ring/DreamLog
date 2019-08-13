using System.Collections.Generic;
using DreamLib.Data.Models;
using System;

namespace DreamLib.Data
{
    public interface IDatalayer
    {
        IEnumerable<DreamLogCollection> DreamLogCollections { get; }
        DreamLogCollection GetDreamLog(long logId);
        bool RemoveDreamLog(long logId);
        bool AddDreamLog(DreamLogCollection log);

        IEnumerable<DreamCategory> DreamCategories { get; }
        DreamCategory GetDreamCategory(long categoryId);
        bool RemoveDreamCategory(long categoryId);
        bool AddDreamCategory(DreamCategory category);

        IEnumerable<DreamLogEntry>  DreamLogEntries { get; }
        IEnumerable<DreamLogEntry> GetLogEntries(DateTime rangeStart, DateTime rangeEnd, long? categoryId);
        IEnumerable<DreamLogEntry> GetLogEntries(DateTime rangeStart, DateTime rangeEnd);
        IEnumerable<DreamLogEntry> GetLogEntries(long? categoryId);
        DreamLogEntry GetLogEntry(long entryId);
        bool RemoveLogEntry(long entryId);
        bool AddLogEntry(DreamLogEntry entry);

        int SaveChanges();
    }
}
