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
    Task<T?> RetrieveById(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an <see cref="IQueryable{T}"/> of all entities
    /// </summary>
    /// <returns><see cref="IQueryable{T}"/> of all entities</returns>
    IQueryable<T> All();

    /// <summary>
    /// Retrieves the entities that match the <paramref name="querySpecification"/>
    /// </summary>
    /// <param name="querySpecification">Query specification</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An asynchronous task containing the list of query results</returns>
    Task<List<T>> Query(QuerySpecification<T> querySpecification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an entity
    /// </summary>
    /// <param name="entity">Entity to add</param>
    void Add(T entity);

    /// <summary>
    /// Saves all changes made in this context to the database
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <returns>
    /// An async task that contains the number of changes that were persisted to the database
    /// </returns>
    Task<int> SaveChanges(CancellationToken cancellationToken = default);
}
