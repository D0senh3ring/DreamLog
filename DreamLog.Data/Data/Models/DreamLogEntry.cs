using DreamLog.Attributes;
using SQLite;
using System;

namespace DreamLog.Data.Models
{
    [Table("dreamlog_entries")]
    public class DreamLogEntry
    {
        [PrimaryKey, AutoIncrement, Column("entry_id")]
        public virtual long EntryId { get; set; }
        [Column("fk_log_id")]
        public virtual long FK_LogId { get; set; }
        [Column("fk_category_id")]
        public virtual long? FK_CategoryId { get; set; }
        [Column("date")]
        public virtual DateTime Date { get; set; }
        [Column("title"), MaxLength(32)]
        public virtual string Title { get; set; }
        [Column("content")]
        public virtual string Content { get; set; }
        [Column("created_at")]
        public virtual DateTime CreatedAt { get; set; }

        [Ignore, CopyIgnore]
        public virtual DreamCategory Category { get; set; }
        [Ignore, CopyIgnore]
        public virtual DreamLogCollection Log { get; set; }
    }
}
