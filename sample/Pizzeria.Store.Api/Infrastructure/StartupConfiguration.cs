using Pizzeria.Store.Data;

namespace Pizzeria.Store.Api.Infrastructure;

internal static class StartupConfiguration
{
    public static IServiceCollection AddStartupConfiguration(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<StoreDbContext>("database", tags: ["live"]);

        return services;
    }
}
