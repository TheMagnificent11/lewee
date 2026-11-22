using Lewee.Application;
using Lewee.Auth.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Auth.Application;

/// <summary>
/// Authentication application configuration.
/// </summary>
public static class AuthApplicationConfiguration
{
    /// <summary>
    /// Adds authentication application services.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>Service collection for chaining.</returns>
    public static IServiceCollection AddLeweeAuthApplication(this IServiceCollection services)
    {
        services.AddApplication(typeof(AuthApplicationConfiguration).Assembly, typeof(User).Assembly);
        return services;
    }
}
