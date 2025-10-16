using Pizzeria.Auth;
using Pizzeria.Common;

namespace Pizzeria.Store.Api.Startup;

internal static class StartupConfiguration
{
    public static IServiceCollection AddStartupConfiguration(this IServiceCollection services)
    {
        services.AddHttpClient<KeycloakHttpClient>(options =>
        {
            options.BaseAddress = new Uri($"http://{ServiceNames.AuthServer}");
            options.Timeout = TimeSpan.FromMinutes(1);
        });

        return services
            .AddTransient<StoreDatabaseConfigurationService>()
            .AddTransient<KeycloakConfigurationService>()
            .AddHostedService<ConfigurationHostedService>();
    }
}
