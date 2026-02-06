using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Lewee.Infrastructure.Fluxor;

/// <summary>
/// WebAssembly API Client Configuration
/// </summary>
public static class WasmApiClientConfiguration
{
    /// <summary>
    /// Adds Refit API Client for WebAssembly
    /// </summary>
    /// <typeparam name="T">API client type</typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="aspireApiServiceName">Aspire API service name</param>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddWebApiHttpClientForWasm<T>(
        this IServiceCollection services,
        string aspireApiServiceName)
        where T : class
    {
        services.AddTransient<WasmAuthTokenDelegatingHandler>();

        services
            .AddRefitClient<T>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri($"https://{aspireApiServiceName}"))
            .AddHttpMessageHandler<WasmAuthTokenDelegatingHandler>();

        return services;
    }
}
