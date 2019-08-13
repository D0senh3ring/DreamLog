using DreamLib.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DreamLib.Data.Models
{
    public class DreamLogEntry
    {
        [Key]
        [Column("entry_id")]
        public virtual long EntryId { get; set; }
        [Column("fk_log_id")]
        public virtual long FK_LogId { get; set; }
        [Column("fk_category_id")]
        public virtual long? FK_CategoryId { get; set; }
        [Column("date")]
        public virtual DateTime Date { get; set; }
        [Column("title")]
        [StringLength(32)]
        public virtual string Title { get; set; }
        [Column("content")]
        public virtual string Content { get; set; }
        [Column("created_at")]
        public virtual DateTime CreatedAt { get; set; }

        [CopyIgnore]
        [ForeignKey("FK_CategoryId")]
        public virtual DreamCategory Category { get; set; }
        [CopyIgnore]
        [ForeignKey("FK_LogId")]
        public virtual DreamLogCollection Log { get; set; }
    }
}
