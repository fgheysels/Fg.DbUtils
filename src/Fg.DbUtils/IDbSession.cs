using System;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Fg.DbUtils
{
    public interface IDbSession : IDbConnection
    {
        /// <summary>
        /// Gets an id that uniquely identifies the IDbSession instance.
        /// </summary>
        Guid SessionId { get; }

        /// <summary>
        /// Gets a value indicating whether the current IDbSession has an active transaction or not.
        /// </summary>
        bool IsInTransaction { get; }

        /// <summary>
        /// Commits the active transaction of this IDbSession.
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// Rollbacks the active transaction of this IDbSession.
        /// </summary>
        void RollbackTransaction();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        void RegisterPostTransactionAction(Action action);

        /// <summary>
        /// Gets the logger that is used by this IDbSession.
        /// </summary>
        /// <remarks>This property is defined to have the possibility to call the Logger in extensions methods.</remarks>
        ILogger<IDbSession> Logger { get; }
    }
}
