using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Lewee.IntegrationTests;

/// <summary>
/// Database Collection
/// </summary>
/// <typeparam name="TDbContextFixture">Database context fixture type</typeparam>
/// <typeparam name="TDbContext">Database context type</typeparam>
/// <typeparam name="TDbSeeder">Database seeder type</typeparam>
public abstract class DataCollection<TDbContextFixture, TDbContext, TDbSeeder> : ICollectionFixture<TDbContextFixture>
    where TDbContextFixture : DatabaseContextFixture<TDbContext, TDbSeeder>, new()
    where TDbContext : DbContext
    where TDbSeeder : IDatabaseSeeder<TDbContext>
{
}
