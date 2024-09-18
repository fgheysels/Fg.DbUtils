using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;

namespace Fg.DbUtils
{
    public class DbSession : IDbSession
    {
        private readonly IDbConnection _connection;
        private readonly DbSessionSettings _settings;
        private readonly ILogger<IDbSession> _logger;

        private readonly Guid _sessionId = Guid.NewGuid();

        Guid IDbSession.SessionId => _sessionId;

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
        public DbSession(IDbConnection connection, ILogger<IDbSession> logger) : this(connection, DbSessionSettings.Default, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbSession"/> class.
        /// </summary>
        /// <param name="connection">The <see cref="IDbConnection"/> that must be used to connect to the database.</param>
        /// <param name="settings">A <see cref="DbSessionSettings"/> instance that defines which settings must be applied.</param>
        /// <param name="logger">An <see cref="ILogger"/> instance that can be used to log traces.</param>
        public DbSession(IDbConnection connection, DbSessionSettings settings, ILogger<IDbSession> logger)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _connection = connection;
            _settings = settings;
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
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();

                _logger.LogDebug("Connection opened on DbSession with Id {DbSessionId}", _sessionId);
            }
        }

        /// <summary>Creates and returns a Command object associated with the connection.</summary>
        /// <returns>A Command object associated with the connection.</returns>
        public IDbCommand CreateCommand()
        {
            var command = _connection.CreateCommand();
            command.CommandTimeout = (int)_settings.CommandTimeout.TotalSeconds;

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

        /// <summary>Begins a database transaction with the specified <see cref="T:System.Data.IsolationLevel"></see> value.</summary>
        /// <param name="isolationLevel">One of the <see cref="T:System.Data.IsolationLevel"></see> values.</param>
        /// <returns>An object representing the new transaction.</returns>
        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            if (IsInTransaction)
            {
                throw new InvalidOperationException("DbSession already has an active transaction");
            }

            Transaction = _connection.BeginTransaction(isolationLevel);

            if (Transaction == null)
            {
                throw new InvalidOperationException("BeginTransaction was not able to start a transaction");
            }

            _logger.LogDebug("Transaction started on DbSession with Id {DbSessionId}", _sessionId);

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

            Transaction.Commit();
            Transaction.Dispose();
            Transaction = null;
            _logger.LogDebug("Transaction committed on DbSession with Id {DbSessionId}", _sessionId);

            while (_postTransactionActions.Count > 0)
            {
                var action = _postTransactionActions.Dequeue();
                action();
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
            Transaction?.Dispose();
            Transaction = null;
            _logger.LogDebug("Transaction rollbacked on DbSession with Id {DbSessionId}", _sessionId);
        }

        private readonly Queue<Action> _postTransactionActions = new Queue<Action>();

        public void RegisterPostTransactionAction(Action action)
        {
            _postTransactionActions.Enqueue(action);
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
            _logger.LogDebug("Disposing DbSession with Id {DbSessionId} ", _sessionId);
            Close();
            _connection?.Dispose();
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
