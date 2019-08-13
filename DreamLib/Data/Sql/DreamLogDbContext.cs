using Microsoft.EntityFrameworkCore;
using DreamLib.Data.Models;

namespace DreamLib.Data.Sql
{
    public sealed class DreamLogDbContext : DbContext
    {
        public DbSet<DreamLogCollection> DreamLogs { get; set; }
        public DbSet<DreamCategory> Categories { get; set; }
        public DbSet<DreamLogEntry> Entries { get; set; }

        private readonly string connectionString;

        public DreamLogDbContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite(this.connectionString);
        }
    }
}
