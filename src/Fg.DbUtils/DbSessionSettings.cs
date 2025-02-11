using System;

namespace Fg.DbUtils
{
    /// <summary>
    /// Defines the settings that must be applied by the DbSessionFactory when a new DbSession instance is created.
    /// </summary>
    public class DbSessionSettings
    {
        public static readonly DbSessionSettings Default = DbSessionSettings.Create(TimeSpan.FromSeconds(30));

        public static DbSessionSettings Create(TimeSpan commandTimeout)
        {
            return new DbSessionSettings()
            {
                CommandTimeout = commandTimeout
            };
        }

        /// <summary>
        /// Gets or sets the wait time before terminating the SQL command that is being executed by the DbSession.
        /// </summary>
        public TimeSpan CommandTimeout { get; private set; } = TimeSpan.FromSeconds(30);
    }
}
