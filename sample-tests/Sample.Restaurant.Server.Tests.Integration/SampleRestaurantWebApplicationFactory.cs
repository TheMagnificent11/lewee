using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sample.Restaurant.Infrastructure.Data;

namespace Sample.Restaurant.Server.Tests.Integration;

public class SampleRestaurantWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string connectionString = $"DataSource={Guid.NewGuid()}.db";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<RestaurantDbContext>))
                        ?? throw new InvalidOperationException("Could not find descriptor for DB context");

            services.Remove(descriptor);

            services.AddDbContext<RestaurantDbContext>(options => options.UseSqlite(this.connectionString));
        });
    }
}
