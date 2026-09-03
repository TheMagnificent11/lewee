using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Auth.Application;

/// <summary>
/// Authorization application configuration.
/// </summary>
/// <remarks>
/// Not every consumer of Lewee requires authorization, so this is a separate, opt-in configuration from
/// <see cref="AuthApplicationConfiguration"/>. Because these pipeline behaviors are registered after the
/// baseline behaviors added by <see cref="Lewee.Application.ApplicationConfiguration.AddPipelineBehaviors"/>,
/// they run after <c>ValidationBehavior</c>.
/// </remarks>
public static class ApplicationAuthConfiguration
{
    /// <summary>
    /// Adds authorization pipeline behaviors.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <returns>Service collection for chaining.</returns>
    public static IServiceCollection AddLeweeApplicationAuth(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TenantLoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AdministratorAuthorizationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TenantRoleAuthorizationBehavior<,>));

        return services;
    }
}
