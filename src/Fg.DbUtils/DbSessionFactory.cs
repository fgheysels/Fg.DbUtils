using System;
using System.Data;

namespace Fg.DbUtils
{
    public class DbSessionFactory : IDbSessionFactory
    {
        private readonly Func<IDbConnection> _dbConnectionFactory;

        public DbSessionFactory(Func<IDbConnection> dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public IDbSession CreateDbSession()
        {
            var session = new DbSession(_dbConnectionFactory());

            session.Open();

            return session;
        }
    }
}
