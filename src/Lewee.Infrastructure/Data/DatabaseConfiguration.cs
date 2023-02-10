using Lewee.Domain;
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
    /// Configures the domain database related to the <typeparamref name="T"/> database context
    /// </summary>
    /// <typeparam name="T">
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
    public static IServiceCollection ConfigureDatabase<T>(
        this IServiceCollection services,
        string connectionString)
        where T : BaseApplicationDbContext<T>
    {
        services.AddDbContextFactory<T>(options => options.UseSqlServer(connectionString));
        services.AddScoped<T>();

        // TODO: figure out how to register each IRepository<TAggregateRoot> with an implementation of Repository<Repository<, T, T>
        ////services.AddTransient(typeof(IRepository<>), typeof(Repository<, T>));

        services.AddSingleton<DomainEventDispatcher<T>>();
        services.AddHostedService<DomainEventDispatcherService<T>>();

        services.AddTransient<IReadModelService, ReadModelService<T>>();

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
