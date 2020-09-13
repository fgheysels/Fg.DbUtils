using System;
using System.Threading.Tasks;

namespace Fg.DbUtils
{
    public static class With
    {
        /// <summary>
        /// Performs an action inside a database-transaction.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="action"></param>
        public static void WithTransaction(this DbSession session, Action action)
        {
            if (session.IsInTransaction)
            {
                action();
            }
            else
            {
                session.BeginTransaction();

                try
                {
                    action();
                    session.CommitTransaction();
                }
                catch
                {
                    session.RollbackTransaction();
                    throw;
                }
            }
        }

        /// <summary>
        /// Performs an asynchronous action inside a database-transaction.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="action"></param>
        public static async Task WithTransactionAsync(this DbSession session, Func<Task> action)
        {
            if (session.IsInTransaction)
            {
                await action();
            }
            else
            {
                session.BeginTransaction();

                try
                {
                    await action();
                    session.CommitTransaction();
                }
                catch
                {
                    session.RollbackTransaction();
                    throw;
                }
            }
        }
    }
}
