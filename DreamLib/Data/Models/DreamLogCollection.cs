using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using DreamLib.Attributes;

namespace DreamLib.Data.Models
{
    public class DreamLogCollection
    {
        [Key]
        [Column("log_id")]
        public virtual long LogId { get; set; }
        [Column("name")]
        [StringLength(32)]
        public virtual string Name { get; set; }

        [CopyIgnore]
        public virtual ICollection<DreamLogEntry> LogEntries { get; set; }
    }
}
