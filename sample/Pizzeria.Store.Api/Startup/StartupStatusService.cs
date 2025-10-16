using Microsoft.Extensions.Caching.Memory;

namespace Pizzeria.Store.Api.Startup;

internal sealed class StartupStatusService
{
    private const string KeycloakReadyKey = "keycloak-ready";
    private const string DatabaseMigratedKey = "database-migrated";
    private const string DatabaseSeededKey = "database-seeded";

    private readonly IMemoryCache memoryCache;
    private readonly ILogger<StartupStatusService> logger;

    public StartupStatusService(
        IMemoryCache memoryCache,
        ILogger<StartupStatusService> logger)
    {
        this.memoryCache = memoryCache;
        this.logger = logger;
    }

    public bool IsKeycloakReady =>
        this.memoryCache.TryGetValue(KeycloakReadyKey, out var keycloakReady) && (bool)keycloakReady!;

    public bool IsDatabaseReady => this.IsDatabaseMigrated && this.IsDatabaseSeeded;

    private bool IsDatabaseMigrated =>
        this.memoryCache.TryGetValue(DatabaseMigratedKey, out var migrated) && (bool)migrated!;

    private bool IsDatabaseSeeded =>
        this.memoryCache.TryGetValue(DatabaseSeededKey, out var seeded) && (bool)seeded!;

    public void SetKeycloakReady()
    {
        this.memoryCache.Set(KeycloakReadyKey, value: true);
        this.logger.LogInformation("Auth server marked as ready");
    }

    public void SetDatabaseMigrated()
    {
        this.memoryCache.Set(DatabaseMigratedKey, value: true);
        this.logger.LogInformation("Database marked as migrated");
    }

    public void SetDatabaseSeeded()
    {
        this.memoryCache.Set(DatabaseSeededKey, value: true);
        this.logger.LogInformation("Database marked as seeded");
    }
}
