namespace Lewee.Infrastructure.Data;

/// <summary>
/// Database Seeder Interface
/// </summary>
public interface IDatabaseSeeder
{
    /// <summary>
    /// Seeds data
    /// </summary>
    /// <returns>Asynchronous task</returns>
    Task Run();
}
