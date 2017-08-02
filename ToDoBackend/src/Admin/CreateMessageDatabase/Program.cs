using System;
using Microsoft.Data.Sqlite;
using Paramore.Brighter.MessageStore.Sqlite;


namespace CreateMessageDatabase
{
    internal class Program
    {
        public const string TableNameMessages = "Messages";

        public static void Main(string[] args)
        {
            Console.WriteLine("Updating {0} with message tables", args[0]);
            SetupMessageDb(args[0]);
            Console.WriteLine("Done");
        }

        private static SqliteConnection SetupMessageDb(string connectionStringPath)
        {
            var connectionString = "DataSource=\"" + connectionStringPath + "\"";
            return CreateDatabaseWithTable(connectionString, SqliteMessageStoreBuilder.GetDDL(TableNameMessages));
        }

        private static SqliteConnection CreateDatabaseWithTable(string dataSourceTestDb, string createTableScript)
        {
            var sqlConnection = new SqliteConnection(dataSourceTestDb);

            sqlConnection.Open();
            using (var command = sqlConnection.CreateCommand())
            {
                command.CommandText = createTableScript;
                command.ExecuteNonQuery();
            }

            return sqlConnection;
        }
     }
}

