using SQLite;

namespace DreamLog.Data.Models
{
    [Table("migration_history")]
    public class MigrationHistory
    {
        [Column("id"), PrimaryKey]
        public string Id { get; set; }
        [Column("timestamp"), NotNull]
        public long Timestamp { get; set; }
    }
}
