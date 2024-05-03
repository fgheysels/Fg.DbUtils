using System;

namespace Fg.DbUtils
{
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

        public TimeSpan CommandTimeout { get; private set; } = TimeSpan.FromSeconds(30);
    }
}
