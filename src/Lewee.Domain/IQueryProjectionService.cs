namespace Lewee.Domain;

/// <summary>
/// Query Projection Service Interface
/// </summary>
public interface IQueryProjectionService
{
    /// <summary>
    /// Retrieves a query projection by its key
    /// </summary>
    /// <typeparam name="T">Query projection type</typeparam>
    /// <param name="key">Read modek key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The query projection if it exists, otherwise null</returns>
    Task<T?> RetrieveByKey<T>(string key, CancellationToken cancellationToken)
        where T : class, IQueryProjection;

    /// <summary>
    /// Persists a query projection to the database
    /// </summary>
    /// <typeparam name="T">Query projection type</typeparam>
    /// <param name="queryProjection">Query projection</param>
    /// <param name="key">Query projection key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An asynchronous task</returns>
    Task AddOrUpdate<T>(T queryProjection, string key, CancellationToken cancellationToken)
        where T : class, IQueryProjection;
}
