using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Database Service Provider Extensions Methods
/// </summary>
public static class DatabaseServiceProviderExtensions
{
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
