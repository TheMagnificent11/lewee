using Lewee.Application.Data;
using Lewee.Infrastructure.Events;
using Microsoft.AspNetCore.Builder;
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
    public static IServiceCollection ConfigureDatabase<TService, TImplementation>(
        this IServiceCollection services,
        string connectionString)
        where TService : class, IDbContext
        where TImplementation : BaseApplicationDbContext<TImplementation>, TService
    {
        services.AddDbContextFactory<TImplementation>(options => options.UseSqlServer(connectionString));
        services.AddScoped<TService>(provider => provider.GetRequiredService<TImplementation>());

        services.AddSingleton<DomainEventDispatcher<TImplementation>>();
        services.AddHostedService<DomainEventDispatcherService<TImplementation>>();

        return services;
    }

    /// <summary>
    /// Migrates the databaes related to the DB context of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">DB context to migrate</typeparam>
    /// <param name="app">Application buider</param>
    public static void MigrationDatabase<T>(this IApplicationBuilder app)
        where T : DbContext
    {
        var service = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

        if (service == null)
        {
            throw new InvalidOperationException("Could not find IServiceScopeFactory");
        }

        using (var serviceScope = service.CreateScope())
        {
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<T>();
            dbContext.Database.Migrate();
        }
    }
}
