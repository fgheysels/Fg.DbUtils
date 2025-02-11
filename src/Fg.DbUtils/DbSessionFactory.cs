using System;
using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Fg.DbUtils
{
    /// <summary>
    /// DbSessionFactory is responsible for creating IDbSession instances.
    /// </summary>
    public class DbSessionFactory : IDbSessionFactory
    {
        private readonly Func<IDbConnection> _dbConnectionFactory;
        private readonly Func<ILogger<IDbSession>> _loggerFactory;

        public DbSessionFactory(Func<IDbConnection> dbConnectionFactory) : this(dbConnectionFactory, () => NullLogger<IDbSession>.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the DbSessionFactory.
        /// </summary>
        /// <param name="dbConnectionFactory">A Func which is responsible for instantiating the IDbConnection that must be used by the IDbSession.</param>
        /// <param name="loggerFactory">A Func which is responsible for creating the ILogger that must be used by the IDbSession.</param>
        public DbSessionFactory(Func<IDbConnection> dbConnectionFactory, Func<ILogger<IDbSession>> loggerFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _loggerFactory = loggerFactory;
        }

        /// <inheritdoc />
        public IDbSession CreateDbSession()
        {
            return CreateDbSession(DbSessionSettings.Default);
        }

        /// <inheritdoc />
        public IDbSession CreateDbSession(DbSessionSettings settings)
        {
            var session = new DbSession(_dbConnectionFactory(), settings, _loggerFactory());

            session.Open();

            return session;
        }
    }
}
