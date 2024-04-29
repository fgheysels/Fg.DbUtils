using System;

namespace Fg.DbUtils
{
    public class DbSessionSettings
    {
        public static readonly DbSessionSettings Default = new DbSessionSettings
        {
            CommandTimeout = TimeSpan.FromSeconds(30)
        };

        public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
