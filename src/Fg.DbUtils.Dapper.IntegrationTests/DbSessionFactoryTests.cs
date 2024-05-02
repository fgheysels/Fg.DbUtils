using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Fg.DbUtils.Dapper.IntegrationTests
{
    public class DbSessionFactoryTests
    {
        [Fact]
        public void DbSessionHasNullLogger_When_LoggerFactoryIsNotSpecified()
        {
            DbSessionFactory factory = new DbSessionFactory(() => SqliteDatabase.OpenDatabase());

            using (var session = factory.CreateDbSession())
            {
                Assert.Equal(NullLogger<IDbSession>.Instance, session.Logger);
            }
        }
    }
}
