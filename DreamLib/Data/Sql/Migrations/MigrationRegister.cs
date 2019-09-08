using System.Collections.Generic;
using DreamLib.Data.Models;
using System.Linq;
using SQLite;
using System;

namespace DreamLib.Data.Sql.Migrations
{
    public class MigrationRegister
    {
        private readonly IEnumerable<DbMigration> migrations;
        private readonly SQLiteConnection connection;

        public MigrationRegister(SQLiteConnection connection)
        {
            this.connection = connection;

            this.CreateMigrationsTableIfNotExists();

            this.migrations = new[]
            {
                new Migration_V1_0_0(connection)
            };
        }

        private void CreateMigrationsTableIfNotExists()
        {
            if (this.connection.Query<object>("SELECT name FROM sqlite_master WHERE type='table' AND name='migration_history'", new object[0]).Count == 0)
            {
                this.connection.CreateTable<MigrationHistory>();
            }
        }

        public void Apply()
        {
            TableQuery<MigrationHistory> history = this.connection.Table<MigrationHistory>();
            foreach(DbMigration migration in this.migrations)
            {
                if(!history.Any(_migration => _migration.Id.Equals(migration.Id))) {
                    migration.Apply();
                    this.connection.Insert(new MigrationHistory() { Id = migration.Id, Timestamp = DateTime.Now.Ticks });
                }
            }
        }
    }
}
