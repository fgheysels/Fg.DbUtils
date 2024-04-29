using System;
using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Fg.DbUtils
{
    public class DbSessionFactory : IDbSessionFactory
    {
        private readonly Func<IDbConnection> _dbConnectionFactory;
        private readonly Func<ILogger<IDbSession>> _loggerFactory;

        public DbSessionFactory(Func<IDbConnection> dbConnectionFactory) : this(dbConnectionFactory, () => NullLogger<IDbSession>.Instance)
        {
        }

        public DbSessionFactory(Func<IDbConnection> dbConnectionFactory, Func<ILogger<IDbSession>> loggerFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _loggerFactory = loggerFactory;
        }

        public IDbSession CreateDbSession()
        {
            var session = new DbSession(_dbConnectionFactory(), _loggerFactory());

            session.Open();

            return session;
        }

        public IDbSession CreateDbSession(DbSessionSettings settings)
        {
            var session = new DbSession(_dbConnectionFactory(), settings, _loggerFactory());

            session.Open();

            return session;
        }
    }
}
