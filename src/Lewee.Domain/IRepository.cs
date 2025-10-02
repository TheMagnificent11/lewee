namespace Lewee.Domain;

/// <summary>
/// Repository interface to access data of <typeparamref name="T"/>
/// </summary>
/// <typeparam name="T">Data type to access</typeparam>
public interface IRepository<T>
    where T : AggregateRoot
{
    /// <summary>
    /// Retrieves the entity with ID <paramref name="id"/>
    /// </summary>
    /// <param name="id">ID of entity to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An asynchronous task that contains the entity if it exist, otherwise the task contains null</returns>
    Task<T?> RetrieveByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of all the entities</returns>
    Task<List<T>> AllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the entities that match the <paramref name="querySpecification"/>
    /// </summary>
    /// <param name="querySpecification">Query specification</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An asynchronous task containing the list of query results</returns>
    Task<List<T>> QueryAsync(QuerySpecification<T> querySpecification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the first entity that matches the <paramref name="querySpecification"/>
    /// </summary>
    /// <param name="querySpecification">Query specification</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An asynchronous task containing the result of the query</returns>
    Task<T?> QueryOneAsync(QuerySpecification<T> querySpecification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an entity
    /// </summary>
    /// <param name="entity">Entity to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes made in this context to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token /// </param>
    /// <returns>
    /// An async task that contains the number of changes that were persisted to the database
    /// </returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
