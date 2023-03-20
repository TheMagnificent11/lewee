using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.IntegrationTests;

/// <summary>
/// Web Application Factory Configuration
/// </summary>
public static class WebApplictionFactoryConfiguration
{
    /// <summary>
    /// REplaces the SQL server dependency injection service descriptor for <typeparamref name="T"/> with an SQL Lite
    /// service descriptor
    /// </summary>
    /// <typeparam name="T">DB context type</typeparam>
    /// <param name="services">Services collection</param>
    /// <param name="usesDbContextFactory">Whether the appliation uses a DB context factory</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if service descriptor cannot be found for <typeparamref name="T"/>
    /// </exception>
    public static void ReplacedSqlServerWithSqlLite<T>(
        this IServiceCollection services,
        bool usesDbContextFactory = true)
        where T : DbContext
    {
        var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<T>))
                        ?? throw new InvalidOperationException("Could not find descriptor for DB context");

        services.Remove(descriptor);

        var connectionString = $"DataSource={typeof(T).Name}-{Guid.NewGuid()}.db";

        if (usesDbContextFactory)
        {
            services.AddDbContextFactory<T>(options => options.UseSqlite(connectionString));
            return;
        }

        services.AddDbContext<T>(options => options.UseSqlite(connectionString));
    }
}
