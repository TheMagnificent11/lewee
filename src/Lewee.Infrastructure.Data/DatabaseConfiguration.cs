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
    /// <param name="domainAssembly">
    /// Assembly containing <see cref="AggregateRoot"/> classes
    /// </param>
    /// <returns>
    /// Services collection for chaining
    /// </returns>
    public static IServiceCollection AddLeweeDatabaseServices<T>(
        this IServiceCollection services,
        Assembly domainAssembly)
        where T : DbContext, IApplicationDbContext
    {
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
    /// <returns>
    /// Services collection for chaining
    /// </returns>
    public static IServiceCollection AddLeweeDatabaseSeeder<TContext, TSeeder>(
        this IServiceCollection services)
        where TContext : DbContext, IApplicationDbContext
        where TSeeder : class, IDatabaseSeeder<TContext>
    {
        return services.AddScoped<IDatabaseSeeder<TContext>, TSeeder>();
    }

    /// <summary>
    /// Migrates the database related to the DB context of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">DB context to migrate</typeparam>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="seedData">Whether to seed data</param>
    /// <returns>Asynchronous task</returns>
    public static async Task MigrateDatabaseAsync<T>(this IServiceProvider serviceProvider, bool seedData = false)
        where T : DbContext
    {
        using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(5)))
        using (var serviceScope = serviceProvider.CreateScope())
        using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<T>())
        {
            var attempt = 0; // Used for exponential back-off

            while (!cancellationTokenSource.IsCancellationRequested)
            {
                var canConnect = await dbContext.Database.CanConnectAsync(cancellationTokenSource.Token);
                if (canConnect)
                {
                    break;
                }

                var delayTimeSpan = CalculateExponentialBackoffDelay(attempt);

                await Task.Delay(delayTimeSpan, cancellationTokenSource.Token);
            }

            if (cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            await dbContext.Database.MigrateAsync(cancellationTokenSource.Token);

            if (!seedData)
            {
                return;
            }

            var seeder = serviceScope.ServiceProvider.GetService<IDatabaseSeeder<T>>();
            if (seeder == null)
            {
                return;
            }

            await seeder.RunAsync(cancellationTokenSource.Token);
        }
    }

    private static TimeSpan CalculateExponentialBackoffDelay(int attempt) =>
        TimeSpan.FromSeconds(Math.Min(Math.Pow(2, attempt), 10));
}
