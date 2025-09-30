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
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddLeweeBlazor<TMapper>(
        this IServiceCollection services,
        Uri serverBaseAddress,
        bool useReduxDevTools)
        where TMapper : class, IMessageToActionMapper
    {
        return services
            .AddTransient<CorrelationIdDelegatingHandler>()
            .AddMessageReceiver<TMapper>(serverBaseAddress)
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
