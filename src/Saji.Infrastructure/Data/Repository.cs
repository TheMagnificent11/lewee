using Microsoft.EntityFrameworkCore;
using Saji.Domain;

namespace Saji.Infrastructure.Data;

/// <summary>
/// Base Repository
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class Repository<T> : IRepository<T>
    where T : class, IEntity
{
    private readonly DbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{T}"/> class
    /// </summary>
    /// <param name="context">Database context</param>
    public Repository(DbContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Gets an <see cref="IQueryable{T}"/> of all entities
    /// </summary>
    /// <returns><see cref="IQueryable{T}"/> of all entities</returns>
    public IQueryable<T> All()
    {
        return this.context.Set<T>();
    }

    /// <summary>
    /// Retrieves the entity with ID <paramref name="id"/>
    /// </summary>
    /// <param name="id">ID of entity to retrieve</param>
    /// <returns>An async task that contains the entity if it exist, otherwise the task contains null</returns>
    public Task<T?> RetrieveById(Guid id)
    {
        return this.All()
            .SingleOrDefaultAsync(x => x.Id == id);
    }
}
