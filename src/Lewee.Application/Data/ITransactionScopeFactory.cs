using Microsoft.EntityFrameworkCore;

namespace Lewee.Application.Data;

/// <summary>
/// Transaction Scope Factory Interface
/// </summary>
/// <typeparam name="TContext">
/// Database context type
/// </typeparam>
public interface ITransactionScopeFactory<TContext>
    where TContext : DbContext
{
    /// <summary>
    /// Creates a database context
    /// </summary>
    /// <returns>
    /// The created database context
    /// </returns>
    TContext CreateContext();

    /// <summary>
    /// Commit
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <returns>
    /// An async task containing the number of changes that were persisted to the database
    /// </returns>
    Task<int> Commit(CancellationToken cancellationToken);

    /// <summary>
    /// Rollback
    /// </summary>
    void Rollback();
}
