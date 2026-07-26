using Correlate.DependencyInjection;
using Correlate.Http;
using Lewee.Common;
using Lewee.Infrastructure.Auth;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Lewee.Infrastructure.Refit;

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
    public static IServiceCollection AddWebApiHttpClient<T>(this IServiceCollection services, string aspireApiServiceName)
        where T : class
    {
        services
            .AddHttpContextAccessor()
            .AddCorrelate(options => options.RequestHeaders = [RequestHeaders.CorrelationId])
            .AddTransient<CorrelatingHttpMessageHandler>()
            .AddTransient<AuthTokenDelegatingHandler>();

        services
            .AddRefitClient<T>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri($"https://{aspireApiServiceName}"))
            .AddHttpMessageHandler<CorrelatingHttpMessageHandler>()
            .AddHttpMessageHandler<AuthTokenDelegatingHandler>();

        return services;
    }
}
