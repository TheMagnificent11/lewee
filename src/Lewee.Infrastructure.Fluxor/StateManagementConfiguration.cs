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
    /// <param name="configureHttpClient">Action to configure the HttpClient for SSE</param>
    /// <returns>Updated services collection</returns>
    public static IServiceCollection AddSseMessageReceiver<TMapper>(
        this IServiceCollection services,
        Action<HttpClient>? configureHttpClient = null)
        where TMapper : class, IMessageToActionMapper
    {
        return services.AddSseMessageReceiver<TMapper>(configureHttpClient, static _ => { });
    }

    /// <summary>
    /// Adds the SSE message receiver configuration and configures its HTTP message handlers
    /// </summary>
    /// <typeparam name="TMapper">Message to action mapper type</typeparam>
    /// <param name="services">Services collection</param>
    /// <param name="configureHttpClient">Action to configure the HttpClient for SSE</param>
    /// <param name="configureHttpClientBuilder">Action to configure the HttpClient builder</param>
    /// <returns>Updated services collection</returns>
    public static IServiceCollection AddSseMessageReceiver<TMapper>(
        this IServiceCollection services,
        Action<HttpClient>? configureHttpClient,
        Action<IHttpClientBuilder> configureHttpClientBuilder)
        where TMapper : class, IMessageToActionMapper
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureHttpClientBuilder);

        services.AddScoped<IMessageToActionMapper, TMapper>();

        var httpClientBuilder = services.AddHttpClient<SseClientMessageReceiver>(client =>
        {
            client.Timeout = System.Threading.Timeout.InfiniteTimeSpan;
            configureHttpClient?.Invoke(client);
        });
        configureHttpClientBuilder(httpClientBuilder);

        return services;
    }
}
