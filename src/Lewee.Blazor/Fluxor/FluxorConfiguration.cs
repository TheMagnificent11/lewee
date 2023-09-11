using System.Reflection;
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
    /// <returns>Updated services collection</returns>
    public static IServiceCollection ConfigureFluxor(this IServiceCollection services)
    {
        services.AddFluxor(options =>
        {
            options.ScanAssemblies(Assembly.GetEntryAssembly(), typeof(ServerHealthState).Assembly);
#if DEBUG
            options.UseReduxDevTools();
#endif
        });

        return services;
    }
}
