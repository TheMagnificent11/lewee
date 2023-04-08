using Lewee.Domain;
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
        where T : ApplicationDbContext<T>
    {
        services.AddDbContextFactory<T>(options => options.UseSqlServer(connectionString));
        services.AddScoped<T>();

        // TODO (https://github.com/TheMagnificent11/lewee/issues/16):
        // Figure out how to register each IRepository<TAggregateRoot> with an implementation of Repository<Repository<, T, T>
        ////services.AddTransient(typeof(IRepository<>), typeof(Repository<, T>));

        services.AddSingleton<DomainEventDispatcher<T>>();
        services.AddHostedService<DomainEventDispatcherService<T>>();

        services.AddTransient<IQueryProjectionService, QueryProjectionService<T>>();

        return services;
    }

    /// <summary>
    /// Configures the domain database related to the <typeparamref name="TContext"/> database context with a data seeder
    /// </summary>
    /// <typeparam name="TContext">
    /// Database context type
    /// </typeparam>
    /// <typeparam name="TSeeder">
    /// Database seeder type
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
    public static IServiceCollection ConfigureDatabaseWithSeeder<TContext, TSeeder>(
        this IServiceCollection services,
        string connectionString)
        where TContext : ApplicationDbContext<TContext>
        where TSeeder : class, IDatabaseSeeder
    {
        var newServices = ConfigureDatabase<TContext>(services, connectionString);

        newServices.AddScoped<IDatabaseSeeder, TSeeder>();

        return newServices;
    }

    /// <summary>
    /// Migrates the databaes related to the DB context of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">DB context to migrate</typeparam>
    /// <param name="app">Application buider</param>
    /// <param name="seedData">Whether to seed data</param>
    /// <returns>Asynchronous task</returns>
    public static async Task MigrationDatabase<T>(this IApplicationBuilder app, bool seedData = false)
        where T : DbContext
    {
        var service = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
            ?? throw new InvalidOperationException("Could not find IServiceScopeFactory");

        using (var serviceScope = service.CreateScope())
        {
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<T>();
            await dbContext.Database.MigrateAsync();

            if (!seedData)
            {
                return;
            }

            var seeder = serviceScope.ServiceProvider.GetService<IDatabaseSeeder>();
            if (seeder == null)
            {
                return;
            }

            await seeder.Run();
        }
    }
}
