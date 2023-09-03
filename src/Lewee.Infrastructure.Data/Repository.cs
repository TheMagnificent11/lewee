using Lewee.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Base Repository
/// </summary>
/// <typeparam name="TAggregate">Aggregate root type</typeparam>
/// <typeparam name="TContext">Database context type</typeparam>
public class Repository<TAggregate, TContext> : IRepository<TAggregate>, IDisposable
    where TAggregate : AggregateRoot
    where TContext : DbContext
{
    private readonly TContext context;

    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{TAggregate, TContext}"/> class
    /// </summary>
    /// <param name="dbContextFactory">Database context factory</param>
    public Repository(IDbContextFactory<TContext> dbContextFactory)
    {
        this.context = dbContextFactory.CreateDbContext();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public IQueryable<TAggregate> All()
    {
        return this.context.Set<TAggregate>();
    }

    /// <inheritdoc />
    public async Task<List<TAggregate>> Query(
        QuerySpecification<TAggregate> querySpecification,
        CancellationToken cancellationToken = default)
    {
        var query = this.ApplySpecification(querySpecification);

        return await query.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<TAggregate?> RetrieveById(Guid id, CancellationToken cancellationToken = default)
    {
        return this.All()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public void Add(TAggregate entity)
    {
        this.context
            .Set<TAggregate>()
            .Add(entity);
    }

    /// <inheritdoc />
    public Task<int> SaveChanges(CancellationToken cancellationToken = default)
    {
        return this.context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Disposes managed resources
    /// </summary>
    /// <param name="disposing">Whether the class is currently disposing</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.context.Dispose();
            }

            this.disposedValue = true;
        }
    }

    private IQueryable<TAggregate> ApplySpecification(QuerySpecification<TAggregate> spec)
    {
        var query = this.context.Set<TAggregate>().AsQueryable();

        if (spec.Criteria != null)
        {
            query = query.Where(spec.Criteria);
        }

        foreach (var include in spec.Includes)
        {
            query = query.Include(include);
        }

        return query;
    }
}
