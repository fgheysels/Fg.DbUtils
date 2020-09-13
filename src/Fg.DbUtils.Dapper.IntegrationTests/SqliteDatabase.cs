using System.Data.SQLite;
using Dapper;

namespace Fg.DbUtils.Dapper.IntegrationTests
{
    internal class SqliteDatabase
    {
        public static SQLiteConnection OpenDatabase()
        {
            var connection = new SQLiteConnection("Data Source=:memory:");

            connection.Open();

            const string createPersonTable =
                "CREATE TABLE 'Persons' \n" +
                "( \n" +
                "  [Id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, \n" +
                "  [FirstName] VARCHAR(50), \n" +
                "  [LastName] VARCHAR(50) \n" +
                ")";

            connection.Execute(createPersonTable);

            return connection;
        }
    }
}
