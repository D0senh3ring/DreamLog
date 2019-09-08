using System.Security.Cryptography;
using System.Text;
using SQLite;
using System;

namespace DreamLib.Data.Sql.Migrations
{
    public abstract class DbMigration
    {
        protected readonly SQLiteConnection connection;

        public DbMigration(SQLiteConnection connection)
        {
            this.connection = connection;
        }
        public abstract string Version { get; }
        public abstract string Id { get; }

        public abstract void Apply();

        protected virtual string GetHash(string content)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content ?? String.Empty);
            using(SHA256Managed sha = new SHA256Managed())
            {
                return Convert.ToBase64String(sha.ComputeHash(bytes));
            }
        }
    }
}
