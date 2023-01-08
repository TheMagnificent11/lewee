using Lewee.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lewee.Infrastructure.Data;

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

    /// <inheritdoc />
    public IQueryable<T> All()
    {
        return this.context.Set<T>();
    }

    /// <inheritdoc />
    public Task<T?> RetrieveById(Guid id)
    {
        return this.All()
            .SingleOrDefaultAsync(x => x.Id == id);
    }
}
