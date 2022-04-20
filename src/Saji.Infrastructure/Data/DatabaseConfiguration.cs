using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Saji.Infrastructure.Data;

/// <summary>
/// Database Configuration
/// </summary>
public static class DatabaseConfiguration
{
    /// <summary>
    /// Migrates the databaes related to the DB context of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">
    /// DB context to migrate
    /// </typeparam>
    /// <param name="builder">
    /// Application buider
    /// </param>
    public static void MigrationDatabase<T>(this IApplicationBuilder builder)
        where T : DbContext
    {
        if (builder?.ApplicationServices == null)
        {
            return;
        }

        var scopeFactory = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>();

        if (scopeFactory == null)
        {
            return;
        }

        using (var serviceScope = scopeFactory.CreateScope())
        {
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<T>();
            dbContext.Database.Migrate();
        }
    }

    /*
    public static void AddDatabaseHealthCheck(this IApplicationBuilder builder, string connectionString)
    {
        if (builder?.ApplicationServices == null)
        {
            return;
        }

        builder.ApplicationServices.AddHealthChecks()
            .AddSqlServer(connectionString);
    }
    */

    /// <summary>
    /// Configures database context and database context factory for a given database context type
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
    public static void ConfigureDatabaseContextAndFactory<T>(
        this IServiceCollection services,
        string connectionString)
        where T : DbContext
    {
        if (services is null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (connectionString is null)
        {
            throw new ArgumentNullException(nameof(connectionString));
        }

        services.AddDbContext<T>(options => options.UseSqlServer(connectionString));

        services.AddDbContextFactory<T>(options => options.UseSqlServer(connectionString));
    }
}