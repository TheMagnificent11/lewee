using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Database Seeder Interface
/// </summary>
/// <typeparam name="TDbContext">Database context type</typeparam>
[SuppressMessage(
    "Major Code Smell",
    "S2326:Unused type parameters should be removed",
    Justification = "Needed for DI purposes")]
public interface IDatabaseSeeder<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    /// Seeds data
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Asynchronous task</returns>
    Task RunAsync(CancellationToken cancellationToken = default);
}
