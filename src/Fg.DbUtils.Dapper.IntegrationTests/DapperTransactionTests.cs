using System;
using System.Data.SQLite;
using Dapper;
using Xunit;

namespace Fg.DbUtils.Dapper.IntegrationTests
{
    public class DapperTransactionTests : IDisposable
    {
        private readonly SQLiteConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="DapperTransactionTests"/> class.
        /// </summary>
        public DapperTransactionTests()
        {
            _connection = SqliteDatabase.OpenDatabase();
        }

        [Fact]
        public void CanExecuteDapperCommandsInTransactionWithoutPassingTransaction()
        {
            var dbSession = new DbSession(_connection);

            dbSession.BeginTransaction();

            try
            {
                dbSession.Execute("INSERT INTO Persons (LastName, FirstName) VALUES (@Name, @FirstName)",
                                  param: new
                                  {
                                      Name = "Gheysels",
                                      FirstName = "Frederik"
                                  });

                throw new InvalidOperationException("The transaction must fail!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                dbSession.RollbackTransaction();
            }
            finally
            {
                var numberOfPersons = dbSession.QuerySingle<int>("SELECT COUNT(*) FROM Persons");
                Assert.Equal(0, numberOfPersons);
            }
        }

        [Fact]
        public void WithTransaction_StartsTransactionIfNecessary()
        {
            var dbSession = new DbSession(_connection);

            Assert.False(dbSession.IsInTransaction);

            dbSession.WithTransaction(() =>
            {
                Assert.True(dbSession.IsInTransaction);
            });
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
