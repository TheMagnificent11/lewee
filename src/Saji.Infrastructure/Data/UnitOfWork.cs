using Microsoft.EntityFrameworkCore;
using Saji.Domain;

namespace Saji.Infrastructure.Data;

/// <summary>
/// Repository
/// </summary>
/// <typeparam name="TContext">
/// Database context type
/// </typeparam>
public class UnitOfWork<TContext> : IUnitOfWork
    where TContext : DbContext
{
    private readonly TContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork{TContext}"/> class
    /// </summary>
    /// <param name="context">
    /// Database context
    /// </param>
    public UnitOfWork(TContext context)
    {
        this.context = context;
    }

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
    public IQueryable<TEntity> GetAll<TEntity, TId>()
        where TEntity : class, IEntity<TId>
        where TId : IComparable, IComparable<TId>, IEquatable<TId>
    {
        return this.context
            .Set<TEntity>()
            .Where(x => true);
    }

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
    public Task<TEntity?> GetByIdAsync<TEntity, TId>(TId id, CancellationToken cancellationToken)
        where TEntity : class, IEntity<TId>
        where TId : IComparable, IComparable<TId>, IEquatable<TId>
    {
        return this.context
            .Set<TEntity>()
            .SingleOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

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
    public Task<List<TEntity>> GetByQueryAsync<TEntity, TId>(
        Func<TEntity, bool> query,
        CancellationToken cancellationToken)
        where TEntity : class, IEntity<TId>
        where TId : IComparable, IComparable<TId>, IEquatable<TId>
    {
        return this.context
            .Set<TEntity>()
            .Where(x => query(x))
            .ToListAsync(cancellationToken);
    }

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
    public void Add<TEntity, TId>(TEntity entity)
        where TEntity : class, IEntity<TId>
        where TId : IComparable, IComparable<TId>, IEquatable<TId>
    {
        this.context
            .Set<TEntity>()
            .Add(entity);
    }

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
    public void Update<TEntity, TId>(TEntity entity)
        where TEntity : class, IEntity<TId>
        where TId : IComparable, IComparable<TId>, IEquatable<TId>
    {
        this.context
            .Set<TEntity>()
            .Update(entity);
    }

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
    public void Delete<TEntity, TId>(TEntity entity)
        where TEntity : class, IEntity<TId>
        where TId : IComparable, IComparable<TId>, IEquatable<TId>
    {
        this.context
            .Set<TEntity>()
            .Remove(entity);
    }

    /// <summary>
    /// Persists changes to the database
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <returns>
    /// Async task containing the number of changes that where persisted
    /// </returns>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return this.context.SaveChangesAsync(cancellationToken);
    }
}
