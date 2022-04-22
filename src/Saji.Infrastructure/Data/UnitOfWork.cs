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
    /// Gets queryable collection of all entities
    /// </summary>
    /// <typeparam name="TEntity">
    /// Entiy type
    /// </typeparam>
    /// <returns>
    /// List of entities
    /// </returns>
    public IQueryable<TEntity> GetAll<TEntity>()
        where TEntity : class
    {
        return this.context
            .Set<TEntity>()
            .Where(x => true);
    }

    /// <summary>
    /// Get an entity by its ID
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
    public Task<TEntity?> GetByIdAsync<TEntity>(Guid id, CancellationToken cancellationToken)
        where TEntity : class, IEntity
    {
        return this.context
            .Set<TEntity>()
            .SingleOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    /// <summary>
    /// Gets an enum entity by its ID
    /// </summary>
    /// <typeparam name="TEnumEntity">
    /// Enum entity type
    /// </typeparam>
    /// <typeparam name="TKey">
    /// Enum type
    /// </typeparam>
    /// <param name="id">
    /// ID of the enum entity to retrieve
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <returns>
    /// An async task containing the enum entity if it exists, otherwise null
    /// </returns>
    public Task<TEnumEntity?> GetByIdAsync<TEnumEntity, TKey>(TKey id, CancellationToken cancellationToken)
        where TEnumEntity : class, IEnumEntity<TKey>
        where TKey : struct, IConvertible, IComparable
    {
        return this.context
            .Set<TEnumEntity>()
            .SingleOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }

    /// <summary>
    /// Gets the entities that satisify the query
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
    public Task<List<TEntity>> GetByQueryAsync<TEntity>(
        Func<TEntity, bool> query,
        CancellationToken cancellationToken)
        where TEntity : class
    {
        return this.context
            .Set<TEntity>()
            .Where(x => query(x))
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Adds a new entity
    /// </summary>
    /// <param name="entity">
    /// Entity
    /// </param>
    /// <typeparam name="TEntity">
    /// Entity type
    /// </typeparam>
    public void Add<TEntity>(TEntity entity)
        where TEntity : class
    {
        this.context
            .Set<TEntity>()
            .Add(entity);
    }

    /// <summary>
    /// Updates the entity
    /// </summary>
    /// <param name="entity">
    /// Entity
    /// </param>
    /// <typeparam name="TEntity">
    /// Entity type
    /// </typeparam>
    public void Update<TEntity>(TEntity entity)
        where TEntity : class
    {
        this.context
            .Set<TEntity>()
            .Update(entity);
    }

    /// <summary>
    /// Deletes the entity
    /// </summary>
    /// <param name="entity">
    /// Entity
    /// </param>
    /// <typeparam name="TEntity">
    /// Entity type
    /// </typeparam>
    public void Delete<TEntity>(TEntity entity)
        where TEntity : class
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
