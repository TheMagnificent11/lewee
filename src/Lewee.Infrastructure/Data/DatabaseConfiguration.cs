using Lewee.Application.Data;
using Lewee.Infrastructure.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Database Configuration
/// </summary>
public static class DatabaseConfiguration
{
    /// <summary>
    /// Configures the domain database related to the <typeparamref name="TImplementation"/> database context
    /// </summary>
    /// <typeparam name="TService">
    /// Database context interface
    /// </typeparam>
    /// <typeparam name="TImplementation">
    /// Database context type
    /// </typeparam>
    /// <param name="services">
    /// Services collection
    /// </param>
    /// <param name="connectionString">
    /// Database connection string
    /// </param>
    /// <returns>
    /// Services collection for chaining
    /// </returns>
    public static IServiceCollection ConfigureDatabase<TService, TImplementation>(this IServiceCollection services, string connectionString)
        where TService : class, IDbContext
        where TImplementation : BaseApplicationDbContext<TImplementation>, TService
    {
        services.AddDbContextFactory<TImplementation>(
            options => options.UseSqlServer(
                connectionString,
                builder => builder.MigrationsAssembly(typeof(TImplementation).Assembly.FullName)));

        services.AddScoped<TService>(provider => provider.GetRequiredService<TImplementation>());

        services.AddScoped<AuditDetailsSaveChangesInterceptor>();
        services.AddScoped<DomainEventSaveChangesInterceptor<TImplementation>>();

        services.AddSingleton<DomainEventDispatcher<TImplementation>>();
        services.AddHostedService<DomainEventDispatcherService<TImplementation>>();

        return services;
    }
}
