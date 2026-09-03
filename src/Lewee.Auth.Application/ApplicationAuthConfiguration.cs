using System.Linq;
using Lewee.Auth.Domain;
using Lewee.Domain;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        services.AddMemoryCache();
        services.DecorateUserRepositoryWithCache();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TenantLoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TenantRoleAuthorizationBehavior<,>));

        return services;
    }

    /// <summary>
    /// Wraps the registered <see cref="IRepository{T}"/> of <see cref="User"/> with <see cref="CachedUserRepository"/>,
    /// so repeated lookups of the same user (e.g. by <see cref="TenantRoleAuthorizationBehavior{TRequest, TResponse}"/>
    /// on every authorized request) do not each require a database round trip.
    /// </summary>
    /// <param name="services">Service collection.</param>
    private static void DecorateUserRepositoryWithCache(this IServiceCollection services)
    {
        var userRepositoryDescriptor = services.LastOrDefault(
            descriptor => descriptor.ServiceType == typeof(IRepository<User>));

        if (userRepositoryDescriptor == null)
        {
            return;
        }

        services.Replace(ServiceDescriptor.Describe(
            typeof(IRepository<User>),
            provider => new CachedUserRepository(
                (IRepository<User>)CreateInnerUserRepository(provider, userRepositoryDescriptor),
                provider.GetRequiredService<IMemoryCache>()),
            userRepositoryDescriptor.Lifetime));
    }

    private static object CreateInnerUserRepository(IServiceProvider provider, ServiceDescriptor descriptor)
    {
        if (descriptor.ImplementationInstance != null)
        {
            return descriptor.ImplementationInstance;
        }

        if (descriptor.ImplementationFactory != null)
        {
            return descriptor.ImplementationFactory(provider);
        }

        return ActivatorUtilities.CreateInstance(provider, descriptor.ImplementationType!);
    }
}
