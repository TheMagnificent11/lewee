using System.Reflection;
using Lewee.Domain;
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
    /// <param name="domainAssembly">
    /// Assembly containing <see cref="AggregateRoot"/> classes
    /// </param>
    /// <returns>
    /// Services collection for chaining
    /// </returns>
    public static IServiceCollection ConfigureDatabase<T>(
        this IServiceCollection services,
        string connectionString,
        Assembly domainAssembly)
        where T : ApplicationDbContext<T>
    {
        services.AddDbContextFactory<T>(options => options.UseSqlServer(connectionString));
        services.AddScoped<T>();

        var aggregateRootType = typeof(AggregateRoot);
        var aggregateRoots = domainAssembly.GetTypes()
            .Where(x => x.IsClass)
            .Where(x => !x.IsAbstract)
            .Where(x => aggregateRootType.IsAssignableFrom(x))
            .ToArray();

        foreach (var ag in aggregateRoots)
        {
            var repositoryIntefaceType = typeof(IRepository<>).MakeGenericType(ag);
            var repositoryType = typeof(Repository<,>).MakeGenericType(ag, typeof(T));
            services.AddTransient(repositoryIntefaceType, repositoryType);
        }

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
    /// <param name="domainAssembly">
    /// Domain assembly
    /// </param>
    /// <returns>
    /// Services collection for chaining
    /// </returns>
    public static IServiceCollection ConfigureDatabaseWithSeeder<TContext, TSeeder>(
        this IServiceCollection services,
        string connectionString,
        Assembly domainAssembly)
        where TContext : ApplicationDbContext<TContext>
        where TSeeder : class, IDatabaseSeeder<TContext>
    {
        var newServices = ConfigureDatabase<TContext>(services, connectionString, domainAssembly);

        newServices.AddScoped<IDatabaseSeeder<TContext>, TSeeder>();

        return newServices;
    }

    /// <summary>
    /// Migrates the database related to the DB context of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">DB context to migrate</typeparam>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="seedData">Whether to seed data</param>
    /// <returns>Asynchronous task</returns>
    public static async Task MigrateDatabase<T>(this IServiceProvider serviceProvider, bool seedData = false)
        where T : DbContext
    {
        using (var serviceScope = serviceProvider.CreateScope())
        {
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<T>();
            await dbContext.Database.MigrateAsync();

            if (!seedData)
            {
                return;
            }

            var seeder = serviceScope.ServiceProvider.GetService<IDatabaseSeeder<T>>();
            if (seeder == null)
            {
                return;
            }

            await seeder.Run();
        }
    }
}
