using Pizzeria.Auth;
using Pizzeria.Common;

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
            .AddCheck<StartupReadinessHealthCheck>("startup_readiness", tags: ["ready"]);

        return services
            .AddTransient<StartupStatusService>()
            .AddTransient<StoreDatabaseConfigurationService>()
            .AddTransient<KeycloakConfigurationService>()
            .AddHostedService<ConfigurationHostedService>();
    }
}
