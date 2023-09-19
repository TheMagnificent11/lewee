using System.Reflection;
using Correlate.DependencyInjection;
using Fluxor;
using Lewee.Blazor.Messaging.Health;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Blazor.Fluxor;

/// <summary>
/// Fluxor Configuration
/// </summary>
public static class FluxorConfiguration
{
    /// <summary>
    /// Configures Fluxor
    /// </summary>
    /// <param name="services">Services collection</param>
    /// <param name="useReduxDevTools">Whether to turn on the Redux Dev Tools</param>
    /// <returns>Updated services collection</returns>
    public static IServiceCollection ConfigureFluxor(this IServiceCollection services, bool useReduxDevTools = false)
    {
        services.AddFluxor(options =>
        {
            options.ScanAssemblies(Assembly.GetEntryAssembly(), typeof(ServerHealthState).Assembly);

            if (useReduxDevTools)
            {
                options.UseReduxDevTools();
            }
        });

        services.AddCorrelate();

        return services;
    }
}
