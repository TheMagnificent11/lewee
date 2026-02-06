using Lewee.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.Auth;

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
    /// <remarks>
    /// This requires <see cref="Microsoft.AspNetCore.Http.IHttpContextAccessor"/> to be registered in the DI container.
    /// Server-side applications should call services.AddHttpContextAccessor() before calling this method.
    /// </remarks>
    public static IServiceCollection AddAuthenticatedUserService(this IServiceCollection services)
    {
        services.AddSingleton<IAuthenticatedUserService, AuthenticatedUserService>();

        return services;
    }
}
