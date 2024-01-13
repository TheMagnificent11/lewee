using Microsoft.EntityFrameworkCore;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Database Seeder Interface
/// </summary>
/// <typeparam name="TDbContext">Database context type</typeparam>
public interface IDatabaseSeeder<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    /// Seeds data
    /// </summary>
    /// <returns>Asynchronous task</returns>
    Task Run();
}
