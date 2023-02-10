namespace Lewee.Domain;

/// <summary>
/// Read Model Service Interface
/// </summary>
public interface IReadModelService
{
    /// <summary>
    /// Retrieves a read model by its key
    /// </summary>
    /// <typeparam name="T">Read model type</typeparam>
    /// <param name="key">Read modek key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The read model if it exists, otherwise null</returns>
    Task<T?> RetrieveByKey<T>(string key, CancellationToken cancellationToken)
        where T : class, IReadModel;

    /// <summary>
    /// Persists a read model to the database
    /// </summary>
    /// <typeparam name="T">Read model type</typeparam>
    /// <param name="readModel">Read model</param>
    /// <param name="key">Read model key</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An asynchronous task</returns>
    Task AddOrUpdate<T>(T readModel, string key, CancellationToken cancellationToken)
        where T : class, IReadModel;
}
