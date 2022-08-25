using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Saji.Infrastructure.Events;

namespace Saji.Infrastructure.Data;

/// <summary>
/// Database Configuration
/// </summary>
public static class DatabaseConfiguration
{
    /// <summary>
    /// Configures the database related to the <typeparamref name="T"/> database context
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
    public static void ConfigureDatabase<T>(this IServiceCollection services, string connectionString)
        where T : DbContext
    {
        services
            .AddDbContext<T>(options => options.UseSqlServer(connectionString));

        services.AddTransient<DomainEventDispatcher<T>>();
        services.AddTransient<TransactionScopeFactory<T>>();
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
