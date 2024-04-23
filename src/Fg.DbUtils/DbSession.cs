using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Data;
using Microsoft.Extensions.Logging.Abstractions;

namespace Fg.DbUtils
{
    public class DbSession : IDbSession
    {
        private readonly IDbConnection _connection;
        private readonly ILogger<IDbSession> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbSession"/> class.
        /// </summary>
        public DbSession(IDbConnection connection) : this(connection, NullLogger<IDbSession>.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbSession"/> class.
        /// </summary>
        /// <param name="connection">The <see cref="IDbConnection"/> that must be used to connect to the database.</param>
        /// <param name="logger">An <see cref="ILogger"/> instance that can be used to log traces.</param>
        public DbSession(IDbConnection connection, ILogger<IDbSession> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        /// <summary>Gets or sets the string used to open a database.</summary>
        /// <returns>A string containing connection settings.</returns>
        public string ConnectionString
        {
            get => _connection.ConnectionString;
            set => _connection.ConnectionString = value;
        }

        /// <summary>Gets the current state of the connection.</summary>
        /// <returns>One of the <see cref="T:System.Data.ConnectionState"></see> values.</returns>
        public ConnectionState State => _connection.State;

        /// <summary>
        /// Gets the current Transaction that is active for this session.
        /// </summary>
        /// <remarks>Returns null if no transaction is active.</remarks>
        public IDbTransaction Transaction { get; private set; }

        /// <summary>
        /// Determines whether or not a Transaction is active for this context.
        /// </summary>
        public bool IsInTransaction => Transaction != null;

        /// <summary>Opens a database connection with the settings specified by the ConnectionString property of the provider-specific Connection object.</summary>
        public void Open()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
        }

        /// <summary>Creates and returns a Command object associated with the connection.</summary>
        /// <returns>A Command object associated with the connection.</returns>
        public IDbCommand CreateCommand()
        {
            var command = _connection.CreateCommand();
            if (IsInTransaction)
            {
                command.Transaction = Transaction;
            }

            return command;
        }

        /// <summary>Begins a database transaction.</summary>
        /// <returns>An object representing the new transaction.</returns>
        public IDbTransaction BeginTransaction()
        {
            return BeginTransaction(IsolationLevel.ReadCommitted);
        }

        private int _nestedTransactionCount = 0;

        /// <summary>Begins a database transaction with the specified <see cref="T:System.Data.IsolationLevel"></see> value.</summary>
        /// <param name="il">One of the <see cref="T:System.Data.IsolationLevel"></see> values.</param>
        /// <returns>An object representing the new transaction.</returns>
        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            if (IsInTransaction)
            {
                _nestedTransactionCount++;
                using (_logger.BeginScope(new Dictionary<string, object>() { ["NestedTransactionCount"] = _nestedTransactionCount }))
                {
                    _logger.LogInformation("BeginTransaction: transaction is already active");
                }
                return Transaction;
            }

            Transaction = _connection.BeginTransaction(il);

            return Transaction;
        }

        /// <summary>
        /// Commits a Transaction
        /// </summary>
        public void CommitTransaction()
        {
            if (IsInTransaction == false)
            {
                return;
            }

            if (_nestedTransactionCount == 0)
            {
                Transaction?.Commit();
                Transaction = null;
                _logger.LogDebug("CommitTransaction: transaction committed");
            }
            else
            {
                _nestedTransactionCount--;
                using (_logger.BeginScope(new Dictionary<string, object>() { ["NestedTransactionCount"] = _nestedTransactionCount }))
                {
                    _logger.LogInformation("CommitTransaction: nested transaction count decremented");
                }
            }
        }

        /// <summary>
        /// Rolls back a Transaction
        /// </summary>
        public void RollbackTransaction()
        {
            if (IsInTransaction == false)
            {
                return;
            }

            Transaction?.Rollback();
            _nestedTransactionCount = 0;
            Transaction = null;
        }

        /// <summary>Changes the current database for an open Connection object.</summary>
        /// <param name="databaseName">The name of the database to use in place of the current database.</param>
        void IDbConnection.ChangeDatabase(string databaseName)
        {
            _connection.ChangeDatabase(databaseName);
        }

        /// <summary>Closes the connection to the database.</summary>
        public void Close()
        {
            RollbackTransaction();
            _connection.Close();

            Transaction = null;
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Transaction?.Dispose();
            _connection?.Dispose();

            Transaction = null;
        }

        /// <summary>Gets the time to wait while trying to establish a connection before terminating the attempt and generating an error.</summary>
        /// <returns>The time (in seconds) to wait for a connection to open. The default value is 15 seconds.</returns>
        int IDbConnection.ConnectionTimeout => _connection.ConnectionTimeout;

        /// <summary>Gets the name of the current database or the database to be used after a connection is opened.</summary>
        /// <returns>The name of the current database or the name of the database to be used once a connection is open. The default value is an empty string.</returns>
        string IDbConnection.Database => _connection.Database;

        ILogger<IDbSession> IDbSession.Logger => _logger;
    }
}
