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
    /// Configures Lewee Blazor
    /// </summary>
    /// <typeparam name="TMapper">Mapper type</typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="serverBaseAddress">Server base address</param>
    /// <param name="useReduxDevTools">Whether to use Redux Dev Tools</param>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection ConfigureLeweeBlazor<TMapper>(
        this IServiceCollection services,
        string serverBaseAddress,
        bool useReduxDevTools)
        where TMapper : class, IMessageToActionMapper
    {
        // TODO: https://github.com/TheMagnificent11/lewee/issues/15
        // LoggingConfiguration.ConfigureLogging(serverBaseAddress);

        return services
            .AddTransient<CorrelationIdDelegatingHandler>()
            .ConfigureMessageReceiver<TMapper>(serverBaseAddress)
            .ConfigureFluxor(useReduxDevTools);
    }

    /// <summary>
    /// Configures the <see cref="CorrelationIdDelegatingHandler" />
    /// </summary>
    /// <param name="builder">HTTP client builder</param>
    /// <returns>The updated HTTP client builder</returns>
    public static IHttpClientBuilder ConfigureCorrelationIdDelegation(this IHttpClientBuilder builder)
    {
        return builder
            .AddHttpMessageHandler<CorrelationIdDelegatingHandler>();
    }
}
