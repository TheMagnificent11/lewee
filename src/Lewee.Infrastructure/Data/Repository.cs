using Lewee.Domain;
using Microsoft.EntityFrameworkCore;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Base Repository
/// </summary>
/// <typeparam name="TAggregate">Aggregate root type</typeparam>
/// <typeparam name="TContext">Databae context type</typeparam>
public class Repository<TAggregate, TContext> : IRepository<TAggregate>, IDisposable
    where TAggregate : class, IAggregateRoot
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
    public Task<TAggregate?> RetrieveById(Guid id, CancellationToken cancellationToken)
    {
        return this.All()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <inheritdoc/>
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

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            this.disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Repository()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }
}
