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
    /// <typeparam name="TId">
    /// ID type
    /// </typeparam>
    /// <returns>
    /// List of entities
    /// </returns>
    IQueryable<TEntity> GetAll<TEntity, TId>()
        where TEntity : class, IEntity<TId>
        where TId : IComparable, IComparable<TId>, IEquatable<TId>;

    /// <summary>
    /// Get entity by ID
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
    /// <typeparam name="TId">
    /// ID type
    /// </typeparam>
    /// <returns>
    /// An async task containing the entity if it exists, otherwise null
    /// </returns>
    Task<TEntity?> GetByIdAsync<TEntity, TId>(TId id, CancellationToken cancellationToken)
        where TEntity : class, IEntity<TId>
        where TId : IComparable, IComparable<TId>, IEquatable<TId>;

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
    /// <typeparam name="TId">
    /// ID type
    /// </typeparam>
    /// <returns>
    /// Async task containing the collection of entities that satisifies the query
    /// </returns>
    Task<List<TEntity>> GetByQueryAsync<TEntity, TId>(
        Func<TEntity, bool> query,
        CancellationToken cancellationToken)
        where TEntity : class, IEntity<TId>
        where TId : IComparable, IComparable<TId>, IEquatable<TId>;

    /// <summary>
    /// Add entity
    /// </summary>
    /// <param name="entity">
    /// Entity
    /// </param>
    /// <typeparam name="TEntity">
    /// Entity type
    /// </typeparam>
    /// <typeparam name="TId">
    /// ID type
    /// </typeparam>
    void Add<TEntity, TId>(TEntity entity)
        where TEntity : class, IEntity<TId>
        where TId : IComparable, IComparable<TId>, IEquatable<TId>;

    /// <summary>
    /// Update entity
    /// </summary>
    /// <param name="entity">
    /// Entity
    /// </param>
    /// <typeparam name="TEntity">
    /// Entity type
    /// </typeparam>
    /// <typeparam name="TId">
    /// ID type
    /// </typeparam>
    void Update<TEntity, TId>(TEntity entity)
        where TEntity : class, IEntity<TId>
        where TId : IComparable, IComparable<TId>, IEquatable<TId>;

    /// <summary>
    /// Delete entity
    /// </summary>
    /// <param name="entity">
    /// Entity
    /// </param>
    /// <typeparam name="TEntity">
    /// Entity type
    /// </typeparam>
    /// <typeparam name="TId">
    /// ID type
    /// </typeparam>
    void Delete<TEntity, TId>(TEntity entity)
        where TEntity : class, IEntity<TId>
        where TId : IComparable, IComparable<TId>, IEquatable<TId>;

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
