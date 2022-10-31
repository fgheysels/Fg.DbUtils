using System.Data;

namespace Fg.DbUtils
{
    internal interface IDbSession : IDbConnection
    {
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
    }
}
