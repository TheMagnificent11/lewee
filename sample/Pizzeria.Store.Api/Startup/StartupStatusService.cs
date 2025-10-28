using Microsoft.Extensions.Caching.Memory;

namespace Pizzeria.Store.Api.Startup;

internal sealed class StartupStatusService
{
    private const string KeycloakReadyKey = "keycloak-ready";

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

    public void SetKeycloakReady()
    {
        this.memoryCache.Set(KeycloakReadyKey, value: true);
        this.logger.LogInformation("Auth server marked as ready");
    }
}
