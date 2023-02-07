using Lewee.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Base Repository
/// </summary>
/// <typeparam name="TAggregate">Aggregate root type</typeparam>
/// <typeparam name="TContext">Databae context type</typeparam>
public class Repository<TAggregate, TContext> : IRepository<TAggregate>
    where TAggregate : class, IAggregateRoot
    where TContext : DbContext
{
    private readonly DbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{TAggregate, TContext}"/> class
    /// </summary>
    /// <param name="context">Database context</param>
    public Repository(TContext context)
    {
        this.context = context;
    }

    /// <inheritdoc />
    public IQueryable<TAggregate> All()
    {
        return this.context.Set<TAggregate>();
    }

    /// <inheritdoc />
    public Task<TAggregate?> RetrieveById(Guid id)
    {
        return this.All()
            .SingleOrDefaultAsync(x => x.Id == id);
    }

    /// <inheritdoc/>
    public Task<int> SaveChanges(CancellationToken cancellationToken = default)
    {
        return this.context.SaveChangesAsync(cancellationToken);
    }
}
