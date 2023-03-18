using Lewee.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.AspNet.Auth;

/// <summary>
/// <see cref="AuthenticatedUserService"/> Configuration
/// </summary>
public static class AuthenticatedUserServiceConfiguration
{
    /// <summary>
    /// Configures the <see cref="AuthenticatedUserService"/>
    /// </summary>
    /// <param name="services">Services collection</param>
    /// <returns>Services collection for chaining</returns>
    public static IServiceCollection ConfigureAuthenticatedUserService(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IAuthenticatedUserService, AuthenticatedUserService>();

        return services;
    }
}
