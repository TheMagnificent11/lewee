using System.Reflection;
using Lewee.Domain;
using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.SqlServer;

/// <summary>
/// SQL Server Configuration
/// </summary>
public static class SqlServerConfiguration
{
    /// <summary>
    /// Configures the domain database related to the <typeparamref name="T"/> database context
    /// </summary>
    /// <typeparam name="T">Database context type</typeparam>
    /// <param name="services">Services collection</param>
    /// <param name="connectionString">Database connection string</param>
    /// <param name="domainAssembly">Assembly containing <see cref="AggregateRoot"/> classes</param>
    /// <returns>
    /// Services collection for chaining
    /// </returns>
    public static IServiceCollection AddSqlServerDatabase<T>(
        this IServiceCollection services,
        string connectionString,
        Assembly domainAssembly)
        where T : ApplicationDbContext<T>
    {
        services.AddDbContextFactory<T>(options => options.UseSqlServer(connectionString));

        return services.AddCommonDatabaseDependencies<T>(domainAssembly);
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
    public static IServiceCollection AddSqlServerDatabaseWithSeeder<TContext, TSeeder>(
        this IServiceCollection services,
        string connectionString,
        Assembly domainAssembly)
        where TContext : ApplicationDbContext<TContext>
        where TSeeder : class, IDatabaseSeeder<TContext>
    {
        var newServices = services.AddSqlServerDatabase<TContext>(connectionString, domainAssembly);

        newServices.AddScoped<IDatabaseSeeder<TContext>, TSeeder>();

        return newServices;
    }
}
