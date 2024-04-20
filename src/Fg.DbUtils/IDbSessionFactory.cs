namespace Fg.DbUtils
{
    public interface IDbSessionFactory
    {
        /// <summary>
        /// Create a new <see cref="IDbSession"/> instance
        /// </summary>
        /// <returns>An <see cref="IDbSession"/></returns>
        IDbSession CreateDbSession();
    }
}
