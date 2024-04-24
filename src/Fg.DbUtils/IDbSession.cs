﻿using System;
using System.Data;
using Microsoft.Extensions.Logging;

namespace Fg.DbUtils
{
    public interface IDbSession : IDbConnection, IDisposable
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

        /// <summary>
        /// Gets the logger that is used by this IDbSession.
        /// </summary>
        /// <remarks>This property is defined to have the possibility to call the Logger in extensions methods.</remarks>
        ILogger<IDbSession> Logger { get; }
    }
}
