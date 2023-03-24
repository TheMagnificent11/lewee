using Respawn;

namespace Lewee.IntegrationTests;

/// <summary>
/// Database Reset Configuration
/// </summary>
public class DatabaseResetConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseResetConfiguration"/> class
    /// </summary>
    /// <param name="dbContextType">Database context type</param>
    /// <param name="respawnerOptions">Options to reset the database</param>
    public DatabaseResetConfiguration(Type dbContextType, RespawnerOptions respawnerOptions)
    {
        this.DbContextType = dbContextType;
        this.RespawnerOptions = respawnerOptions;
    }

    /// <summary>
    /// Gets the DB context type
    /// </summary>
    public Type DbContextType { get; }

    /// <summary>
    /// Gets the options to reset the database
    /// </summary>
    public RespawnerOptions RespawnerOptions { get; }
}
