using System.Reflection;
using Lewee.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Common Database Configuration Extension Methods
/// </summary>
public static class CommonDatabaseConfigurationExtensions
{
    /// <summary>
    /// Adds database dependencies common to all supported database providers
    /// </summary>
    /// <typeparam name="T">Application database context type</typeparam>
    /// <param name="services">Services collection</param>
    /// <param name="domainAssembly">Assembly containing <see cref="AggregateRoot"/> classes</param>
    /// <returns>The updated service collection</returns>
    public static IServiceCollection AddCommonDatabaseDependencies<T>(
        this IServiceCollection services,
        Assembly domainAssembly)
        where T : ApplicationDbContext<T>
    {
        services.AddScoped<T>();

        var aggregateRootType = typeof(AggregateRoot);
        var aggregateRoots = domainAssembly.GetTypes()
            .Where(x => x.IsClass)
            .Where(x => !x.IsAbstract)
            .Where(x => aggregateRootType.IsAssignableFrom(x))
            .ToArray();

        foreach (var ag in aggregateRoots)
        {
            var repositoryInterfaceType = typeof(IRepository<>).MakeGenericType(ag);
            var repositoryType = typeof(Repository<,>).MakeGenericType(ag, typeof(T));
            services.AddTransient(repositoryInterfaceType, repositoryType);
        }

        services.AddSingleton<DomainEventDispatcher<T>>();
        services.AddHostedService<DomainEventDispatcherService<T>>();

        services.AddTransient<IQueryProjectionService, QueryProjectionService<T>>();

        return services;
    }
}
