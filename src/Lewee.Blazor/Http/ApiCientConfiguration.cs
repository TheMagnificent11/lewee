using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Lewee.Blazor.Http;

/// <summary>
/// API Client Configuration
/// </summary>
public static class ApiCientConfiguration
{
    /// <summary>
    /// Adds Refit API Client
    /// </summary>
    /// <typeparam name="T">API client type</typeparam>
    /// <param name="services">Service collection</param>
    /// <param name="aspireApiServiceName">Aspire API service name</param>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddApiClient<T>(this IServiceCollection services, string aspireApiServiceName)
        where T : class
    {
        services.AddHttpContextAccessor();
        services.AddTransient<AuthTokenDelegatingHandler>();

        services
            .AddRefitClient<T>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri($"https://{aspireApiServiceName}"))
            .AddHttpMessageHandler<AuthTokenDelegatingHandler>()
            .AddHttpMessageHandler<CorrelationIdDelegatingHandler>();

        return services;
    }
}
