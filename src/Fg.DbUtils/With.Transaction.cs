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
        public static void WithTransaction(this IDbSession session, Action action)
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
        /// Performs an action inside a database-transaction that returns a result.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="action"></param>
        public static TResult WithTransaction<TResult>(this IDbSession session, Func<TResult> action)
        {
            if (session.IsInTransaction)
            {
                return action();
            }
            else
            {
                TResult result;

                session.BeginTransaction();

                try
                {
                    result = action();
                    session.CommitTransaction();
                }
                catch
                {
                    session.RollbackTransaction();
                    throw;
                }

                return result;
            }
        }

        /// <summary>
        /// Performs an asynchronous action inside a database-transaction.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="action"></param>
        public static async Task WithTransactionAsync(this IDbSession session, Func<Task> action)
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

        /// <summary>
        /// Performs an asynchronous action inside a database-transaction that returns a result.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="action"></param>
        public static async Task<TResult> WithTransactionAsync<TResult>(this IDbSession session, Func<Task<TResult>> action)
        {            
            if (session.IsInTransaction)
            {
                var result = await action();
                return result;
            }
            else
            {
                TResult result;

                session.BeginTransaction();

                try
                {
                    result = await action();
                    session.CommitTransaction();
                }
                catch
                {
                    session.RollbackTransaction();
                    throw;
                }

                return result;
            }
        }
    }
}
