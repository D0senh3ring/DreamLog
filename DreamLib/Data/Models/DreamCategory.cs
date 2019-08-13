using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using DreamLib.Attributes;

namespace DreamLib.Data.Models
{
    public class DreamCategory
    {
        [Key]
        [Column("category_id")]
        public virtual long CategoryId { get; set; }
        [StringLength(32)]
        [Column("color_string")]
        public virtual string ColorString { get; set; }
        [Column("name")]
        [StringLength(32)]
        public virtual string Name { get; set; }

        [CopyIgnore]
        public virtual ICollection<DreamLogEntry> DreamLogEntries { get; set; }
    }
}
