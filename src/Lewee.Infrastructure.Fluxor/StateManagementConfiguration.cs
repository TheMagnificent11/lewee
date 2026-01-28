using System.Reflection;
using Correlate.DependencyInjection;
using Fluxor;
using Fluxor.Blazor.Web.ReduxDevTools;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.Fluxor;

/// <summary>
/// Fluxor Configuration
/// </summary>
public static class StateManagementConfiguration
{
    /// <summary>
    /// Configures Fluxor
    /// </summary>
    /// <param name="services">Services collection</param>
    /// <param name="useReduxDevTools">Whether to turn on the Redux Dev Tools</param>
    /// <param name="statemanagementAssemblies">Fluxor statement management assemblies</param>
    /// <returns>Updated services collection</returns>
    public static IServiceCollection AddLeweeFluxor(
        this IServiceCollection services,
        bool useReduxDevTools = false,
        params Assembly[] statemanagementAssemblies)
    {
        services.AddFluxor(options =>
        {
            options.ScanAssemblies(Assembly.GetEntryAssembly(), statemanagementAssemblies);

            if (useReduxDevTools)
            {
                options.UseReduxDevTools();
            }
        });

        services.AddCorrelate();

        return services;
    }

    /// <summary>
    /// Adds the SSE message receiver configuration
    /// </summary>
    /// <typeparam name="TMapper">Message to action mapper type</typeparam>
    /// <param name="services">Services collection</param>
    /// <returns>Updated services collection</returns>
    public static IServiceCollection AddSseMessageReceiver<TMapper>(this IServiceCollection services)
        where TMapper : class, IMessageToActionMapper
    {
        services.AddScoped<IMessageToActionMapper, TMapper>();

        return services;
    }
}
