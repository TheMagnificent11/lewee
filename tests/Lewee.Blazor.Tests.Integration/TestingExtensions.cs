using Lewee.Blazor.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Blazor.Tests.Integration;

/// <summary>
/// Extensions for testing Lewee Blazor components
/// </summary>
public static class TestingExtensions
{
    /// <summary>
    /// Adds and configures Blazor with a Fluxor and SignalR message handling for integration testing
    /// </summary>
    /// <typeparam name="TMapper">Mapper type</typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="testHttpClient">Test HTTP client from TestServer</param>
    /// <param name="useReduxDevTools">Whether to use Redux Dev Tools</param>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddLeweeBlazorForTesting<TMapper>(
        this IServiceCollection services,
        HttpClient testHttpClient,
        bool useReduxDevTools = false)
        where TMapper : class, IMessageToActionMapper
    {
        var testHandler = new TestHttpMessageHandler(testHttpClient);
        return services.AddLeweeBlazor<TMapper>(
            testHttpClient.BaseAddress!,
            useReduxDevTools,
            testHandler);
    }
}
