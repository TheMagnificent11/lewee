using Pizzeria.Auth;
using Pizzeria.Common;
using Pizzeria.Store.Data;

namespace Pizzeria.Store.Api.Startup;

internal static class StartupConfiguration
{
    public static IServiceCollection AddStartupConfiguration(this IServiceCollection services)
    {
        services.AddMemoryCache();

        services.AddHttpClient<KeycloakHttpClient>(options =>
        {
            options.BaseAddress = new Uri($"http://{ServiceNames.AuthServer}");
            options.Timeout = TimeSpan.FromMinutes(1);
        });

        services.AddHealthChecks()
            .AddDbContextCheck<StoreDbContext>("database", tags: ["live"])
            .AddCheck<StartupReadinessHealthCheck>("startup_readiness", tags: ["ready"]);

        return services
            .AddTransient<StartupStatusService>()
            .AddTransient<AuthServerConfigurationService>()
            .AddHostedService<ConfigurationHostedService>();
    }
}
