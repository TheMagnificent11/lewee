namespace Pizzeria.Configuration.Services;

/// <summary>
/// Database configuration service
/// </summary>
public interface IDatabaseConfigurationService
{
    /// <summary>
    /// Configures the database
    /// </summary>
    /// <returns>Task</returns>
    Task ConfigureAsync();
}
