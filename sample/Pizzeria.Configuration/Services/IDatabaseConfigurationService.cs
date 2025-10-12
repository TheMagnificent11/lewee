namespace Pizzeria.Configuration.Services;

/// <summary>
/// Database configuration service
/// </summary>
public interface IDatabaseConfigurationService
{
    /// <summary>
    /// Migrates the database
    /// </summary>
    /// <returns>Task</returns>
    Task MigrateAsync();

    /// <summary>
    /// Seeds the database with initial data
    /// </summary>
    /// <returns>Task</returns>
    Task SeedDataAsync();
}
