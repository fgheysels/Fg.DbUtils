using System;
using System.Data.SQLite;
using Dapper;
using Xunit;

namespace Fg.DbUtils.Dapper.IntegrationTests
{
    public class DbSessionTransactionTests : IDisposable
    {
        private readonly SQLiteConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbSessionTransactionTests"/> class.
        /// </summary>
        public DbSessionTransactionTests()
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

        [Fact]
        public void WithTransaction_CanReturnResult()
        {
            var dbSession = new DbSession(_connection);

            int result = dbSession.WithTransaction(() =>
            {
                return 7;
            });

            Assert.Equal(7, result);
        }

        [Fact]
        public void BeginTransaction_Throws_WhenTransactionAlreadyActive()
        {
            var dbSession = new DbSession(_connection);

            dbSession.BeginTransaction();

            Assert.Throws<InvalidOperationException>(() => dbSession.BeginTransaction());
        }

        [Fact]
        public void CommitTransaction_Executes_RegisteredPostTransactionActions()
        {
            var dbSession = new DbSession(_connection);

            dbSession.BeginTransaction();

            bool postTransactionExecuted = false;

            dbSession.RegisterPostTransactionAction(() => postTransactionExecuted = true);

            dbSession.CommitTransaction();

            Assert.True(postTransactionExecuted);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
