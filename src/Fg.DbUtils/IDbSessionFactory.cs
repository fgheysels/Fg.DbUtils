namespace Fg.DbUtils
{
    public interface IDbSessionFactory
    {
        /// <summary>
        /// Create a new <see cref="IDbSession"/> instance
        /// </summary>
        /// <returns>An <see cref="IDbSession"/></returns>
        IDbSession CreateDbSession();

        /// <summary>
        /// Create a new <see cref="IDbSession"/> instance
        /// </summary>
        /// <param name="settings">The <see cref="DbSessionSettings"/> instance that defines the settings that must
        /// be applied to the <see cref="IDbSession"/> that is returned.</param>
        /// <returns>An <see cref="IDbSession"/></returns>
        IDbSession CreateDbSession(DbSessionSettings settings);
    }
}
