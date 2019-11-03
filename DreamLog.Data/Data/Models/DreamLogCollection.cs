using System.Collections.Generic;
using DreamLog.Attributes;
using SQLite;

namespace DreamLog.Data.Models
{
    [Table("dreamlog_collections")]
    public class DreamLogCollection
    {
        [PrimaryKey, AutoIncrement, Column("log_id")]
        public virtual long LogId { get; set; }
        [Column("name"), MaxLength(32)]
        public virtual string Name { get; set; }

        [Ignore, CopyIgnore]
        public virtual ICollection<DreamLogEntry> LogEntries { get; set; } = new List<DreamLogEntry>();
    }
}
