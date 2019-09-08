using System.Collections.Generic;
using DreamLib.Attributes;
using SQLite;

namespace DreamLib.Data.Models
{
    [Table("dreamlog_categories")]
    public class DreamCategory
    {
        [PrimaryKey, AutoIncrement, Column("category_id")]
        public virtual long CategoryId { get; set; }
        [MaxLength(32), Column("color_string")]
        public virtual string ColorString { get; set; }
        [MaxLength(32), Column("name")]
        public virtual string Name { get; set; }

        [Ignore, CopyIgnore]
        public virtual ICollection<DreamLogEntry> DreamLogEntries { get; set; } = new List<DreamLogEntry>();
    }
}
