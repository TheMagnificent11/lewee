using Lewee.Blazor.Fluxor;
using Lewee.Blazor.Http;
using Lewee.Blazor.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Blazor;

/// <summary>
/// Lewee Blazor Configuration
/// </summary>
public static class Configuration
{
    /// <summary>
    /// Adds and configures Blazor with a Fluxor and SignalR message handling
    /// </summary>
    /// <typeparam name="TMapper">Mapper type</typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="serverBaseAddress">Server base address</param>
    /// <param name="useReduxDevTools">Whether to use Redux Dev Tools</param>
    /// <param name="httpMessageHandler">Optional HTTP message handler for testing scenarios</param>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddLeweeBlazor<TMapper>(
        this IServiceCollection services,
        Uri serverBaseAddress,
        bool useReduxDevTools,
        HttpMessageHandler? httpMessageHandler)
        where TMapper : class, IMessageToActionMapper
    {
        return services
            .AddTransient<CorrelationIdDelegatingHandler>()
            .AddMessageReceiver<TMapper>(serverBaseAddress, httpMessageHandler)
            .AddFluxor(useReduxDevTools);
    }

    /// <summary>
    /// Adds and configures Blazor with a Fluxor and SignalR message handling using service discovery
    /// </summary>
    /// <typeparam name="TMapper">Mapper type</typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="apiAspireServiceName">Name of the HttpClient configured with service discovery</param>
    /// <param name="useReduxDevTools">Whether to use Redux Dev Tools</param>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddLeweeBlazor<TMapper>(
        this IServiceCollection services,
        string apiAspireServiceName,
        bool useReduxDevTools)
        where TMapper : class, IMessageToActionMapper
    {
        return services
            .AddTransient<CorrelationIdDelegatingHandler>()
            .AddMessageReceiverWithServiceDiscovery<TMapper>(apiAspireServiceName)
            .AddFluxor(useReduxDevTools);
    }

    /// <summary>
    /// Configures the <see cref="CorrelationIdDelegatingHandler" />
    /// </summary>
    /// <param name="builder">HTTP client builder</param>
    /// <returns>The updated HTTP client builder</returns>
    public static IHttpClientBuilder AddCorrelationIdDelegationHandler(this IHttpClientBuilder builder)
    {
        return builder
            .AddHttpMessageHandler<CorrelationIdDelegatingHandler>();
    }
}
