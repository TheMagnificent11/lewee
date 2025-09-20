using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Sample.Restaurant.Infrastructure.Data;

namespace Sample.Restaurant.Server.Tests.Integration;

public class RestaurantWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly RestaurantDbContextFixture dbContextFixture;

    public RestaurantWebApplicationFactory(RestaurantDbContextFixture dbContextFixture)
    {
        this.dbContextFixture = dbContextFixture;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Add in-memory configuration to override the database connection string
            var inMemoryConfig = new Dictionary<string, string>
            {
                ["ConnectionStrings:Sample.Restaurant"] = this.dbContextFixture.ConnectionString
            };
            config.AddInMemoryCollection(inMemoryConfig!);
        });

        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContextFactory registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDbContextFactory<RestaurantDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Register a new DbContextFactory that uses the TestContainer connection string
            services.AddDbContextFactory<RestaurantDbContext>(options =>
                options.UseSqlServer(this.dbContextFixture.ConnectionString));
        });
    }
}
