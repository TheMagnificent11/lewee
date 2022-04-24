using Microsoft.EntityFrameworkCore;

namespace Saji.Infrastructure.Data;

/// <summary>
/// Transaction Scope Factory
/// </summary>
/// <typeparam name="TContext">
/// Database context
/// </typeparam>
public class TransactionScopeFactory<TContext> : IDisposable
    where TContext : DbContext
{
    private readonly IDbContextFactory<TContext> contextFactory;

    private bool isCompleted = false;
    private TContext? context;

    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionScopeFactory{TContext}"/> class
    /// </summary>
    /// <param name="contextFactory">
    /// Database context factory
    /// </param>
    public TransactionScopeFactory(IDbContextFactory<TContext> contextFactory)
    {
        this.contextFactory = contextFactory;
    }

    /// <summary>
    /// Disponses the instance of <see cref="TransactionScopeFactory{TContext}"/>
    /// </summary>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Creates a database context
    /// </summary>
    /// <returns>
    /// The created database context
    /// </returns>
    public TContext CreateContext()
    {
        if (this.context != null)
        {
            throw new InvalidOperationException("Database context has already been created");
        }

        this.context = this.contextFactory.CreateDbContext();

        return this.context;
    }

    /// <summary>
    /// Commit
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <returns>
    /// An async task containing the number of changes that were persisted to the database
    /// </returns>
    public Task<int> Commit(CancellationToken cancellationToken)
    {
        this.ValidateContext();

        this.isCompleted = true;

#pragma warning disable CS8602 // Dereference of a possibly null reference. Check done in ValidateContext().
        return this.context.SaveChangesAsync(cancellationToken);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }

    /// <summary>
    /// Rollback
    /// </summary>
    public void Rollback()
    {
        this.ValidateContext();

        this.isCompleted = true;

#pragma warning disable CS8602 // Dereference of a possibly null reference. Check done in ValidateContext().
        this.context.Dispose();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        this.context = null;
    }

    /// <summary>
    /// Disponses the instance of <see cref="TransactionScopeFactory{TContext}"/>
    /// </summary>
    /// <param name="disposing">
    /// Whether disposal is currently taking place
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                // dispose managed state (managed objects)
                if (this.context != null)
                {
                    this.context.Dispose();
                    this.context = null;
                }

                /*
                 * Not disposing IDbContextFactory because it should be disposed by its creator
                 */
            }

            /*
             * free unmanaged resources (unmanaged objects) and override finalizer
             * set large fields to null
             */

            this.disposedValue = true;
        }
    }

    private void ValidateContext()
    {
        if (this.context == null)
        {
            throw new InvalidOperationException("A transaction scope has not been created. Call `CreateContext` first");
        }

        if (this.isCompleted)
        {
            throw new InvalidOperationException("Transaction has already been committed or rolled back");
        }
    }
}
