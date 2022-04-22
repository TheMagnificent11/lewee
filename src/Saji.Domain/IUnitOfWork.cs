namespace Saji.Domain;

/// <summary>
/// Unit Of Work Interface
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Get all entities
    /// </summary>
    /// <typeparam name="TEntity">
    /// Entiy type
    /// </typeparam>
    /// <returns>
    /// List of entities
    /// </returns>
    IQueryable<TEntity> GetAll<TEntity>()
        where TEntity : class;

    /// <summary>
    /// Gets an entity by ID
    /// </summary>
    /// <param name="id">
    /// ID of entity to lookup
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <typeparam name="TEntity">
    /// Entity type
    /// </typeparam>
    /// <returns>
    /// An async task containing the entity if it exists, otherwise null
    /// </returns>
    Task<TEntity?> GetByIdAsync<TEntity>(Guid id, CancellationToken cancellationToken)
        where TEntity : class, IEntity;

    /// <summary>
    /// Gets an enum entity by IS
    /// </summary>
    /// <typeparam name="TEnumEntity">
    /// Enum entity type
    /// </typeparam>
    /// <typeparam name="TKey">
    /// Enum type
    /// </typeparam>
    /// <param name="id">
    /// ID of enum entity to retrieve
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <returns>
    /// An async tass containing the enum entity if it exists, otherwise null
    /// </returns>
    Task<TEnumEntity?> GetByIdAsync<TEnumEntity, TKey>(TKey id, CancellationToken cancellationToken)
        where TEnumEntity : class, IEnumEntity<TKey>
        where TKey : struct, IConvertible, IComparable;

    /// <summary>
    /// Gets the entities that satisifies the query
    /// </summary>
    /// <param name="query">
    /// Query
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <typeparam name="TEntity">
    /// Entity type
    /// </typeparam>
    /// <returns>
    /// Async task containing the collection of entities that satisifies the query
    /// </returns>
    Task<List<TEntity>> GetByQueryAsync<TEntity>(
        Func<TEntity, bool> query,
        CancellationToken cancellationToken)
        where TEntity : class;

    /// <summary>
    /// Adds an entity
    /// </summary>
    /// <param name="entity">
    /// Entity to add
    /// </param>
    /// <typeparam name="TEntity">
    /// Entity type
    /// </typeparam>
    void Add<TEntity>(TEntity entity)
        where TEntity : class;

    /// <summary>
    /// Updates an entity
    /// </summary>
    /// <param name="entity">
    /// Entity to update
    /// </param>
    /// <typeparam name="TEntity">
    /// Entity type
    /// </typeparam>
    void Update<TEntity>(TEntity entity)
        where TEntity : class;

    /// <summary>
    /// Delete entity
    /// </summary>
    /// <param name="entity">
    /// Entity
    /// </param>
    /// <typeparam name="TEntity">
    /// Entity type
    /// </typeparam>
    void Delete<TEntity>(TEntity entity)
        where TEntity : class;

    /// <summary>
    /// Persists changes to the database
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <returns>
    /// Async task containing the number of changes that where persisted
    /// </returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
