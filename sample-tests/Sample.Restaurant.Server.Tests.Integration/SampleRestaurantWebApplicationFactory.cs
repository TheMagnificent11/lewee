using Lewee.IntegrationTests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Sample.Restaurant.Infrastructure.Data;

namespace Sample.Restaurant.Server.Tests.Integration;

public class SampleRestaurantWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services => services.ReplacedSqlServerWithSqlLite<RestaurantDbContext>());
    }
}
