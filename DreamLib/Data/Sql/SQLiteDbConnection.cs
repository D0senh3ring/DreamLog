using DreamLib.Data.Sql.Migrations;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamLib.Data.Sql
{
    public class SQLiteDbConnection
    {
        private readonly string filePath;

        private SQLiteConnection connection;

        public SQLiteDbConnection(string filePath)
        {
            this.filePath = filePath;
        }

        public void Open()
        {
            if (!this.IsOpen)
            {
                this.connection = new SQLiteConnection(filePath);

                new MigrationRegister(this.connection).Apply();
            }
        }

        public TableQuery<T> GetEntities<T>() where T : new()
        {
            if (this.IsOpen)
                return this.connection.Table<T>();
            return null;
        }

        public bool AddEntity<T>(T insert) where T : new()
        {
            if (this.IsOpen)
                return this.connection.Insert(insert) == 1;
            return false;
        }

        public bool DeleteEntity<T>(object primaryKey) where T : new()
        {
            if (this.IsOpen)
                return this.connection.Delete<T>(primaryKey) == 1;
            return false;
        }

        public int UpdateAll<T>(IEnumerable<T> items) where T : new()
        {
            if(this.IsOpen)
                return this.connection.UpdateAll(items);
            return 0;
        }

        public SQLiteCommand CreateCommand(string commandText, object[] parameters)
        {
            return this.connection.CreateCommand(commandText, parameters);
        }

        public bool IsOpen
        {
            get { return !(this.connection is null); }
        }

        public void Close()
        {
            if(this.IsOpen)
                this.connection.Close();
        }
    }
}
