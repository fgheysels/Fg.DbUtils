﻿using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Fg.DbUtils
{
    public static class With
    {
        /// <summary>
        /// Performs an action inside a database-transaction with a ReadCommitted isolation level.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="action"></param>
        public static void WithTransaction(this IDbSession session, Action action)
        {
            WithTransaction(session, IsolationLevel.ReadCommitted, action);
        }

        /// <summary>
        /// Performs an action inside a database-transaction.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="isolationLevel">The <see cref="IsolationLevel"/> that must be used for this transaction.</param>
        /// <param name="action"></param>
        public static void WithTransaction(this IDbSession session, IsolationLevel isolationLevel, Action action)
        {
            session.BeginTransaction(isolationLevel);

            try
            {
                action();
                session.CommitTransaction();
            }
            catch (Exception ex)
            {
                session.Logger.LogError(ex, "An exception occurred while performing database-operations in a transaction.");

                session.RollbackTransaction();
                throw;
            }
        }

        /// <summary>
        /// Performs an action that returns a result inside a database-transaction with a ReadCommitted isolation level.
        /// </summary>
        /// <param name="session">The IDbSession for which a transaction must be started.</param>
        /// <param name="action">The action that must be performed.</param>
        public static TResult WithTransaction<TResult>(this IDbSession session, Func<TResult> action)
        {
            return WithTransaction(session, IsolationLevel.ReadCommitted, action);
        }

        /// <summary>
        /// Performs an action inside a database-transaction that returns a result.
        /// </summary>
        /// <param name="session">The IDbSession for which a transaction must be started.</param>
        /// <param name="isolationLevel">The <see cref="IsolationLevel"/> that must be used for this transaction.</param>
        /// <param name="action">The action that must be performed.</param>
        public static TResult WithTransaction<TResult>(this IDbSession session, IsolationLevel isolationLevel, Func<TResult> action)
        {
            if (session.IsInTransaction)
            {
                return action();
            }
            else
            {
                TResult result;

                session.BeginTransaction(isolationLevel);

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
            await WithTransactionAsync(session, IsolationLevel.ReadCommitted, action);
        }

        /// <summary>
        /// Performs an asynchronous action inside a database-transaction.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="isolationLevel">The <see cref="IsolationLevel"/> that must be used for this transaction.</param>
        /// <param name="action"></param>
        public static async Task WithTransactionAsync(this IDbSession session, IsolationLevel isolationLevel, Func<Task> action)
        {
            session.BeginTransaction(isolationLevel);

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

        /// <summary>
        /// Performs an asynchronous action inside a database-transaction that returns a result.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="action"></param>
        public static async Task<TResult> WithTransactionAsync<TResult>(this IDbSession session, Func<Task<TResult>> action)
        {
            return await WithTransactionAsync(session, IsolationLevel.ReadCommitted, action);
        }

        /// <summary>
        /// Performs an asynchronous action inside a database-transaction that returns a result.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="isolationLevel">The <see cref="IsolationLevel"/> that must be used for this transaction.</param>
        /// <param name="action"></param>
        public static async Task<TResult> WithTransactionAsync<TResult>(this IDbSession session, IsolationLevel isolationLevel, Func<Task<TResult>> action)
        {
            TResult result;

            session.BeginTransaction(isolationLevel);

            try
            {
                result = await action();
                session.CommitTransaction();
            }
            catch (Exception ex)
            {
                session.Logger.LogError(ex, "An exception occurred while performing database-operations in a transaction.");

                session.RollbackTransaction();
                throw;
            }

            return result;
        }
    }
}
