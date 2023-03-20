using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Sample.Restaurant.Server.Tests.Integration;

public class RestaurantWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var projectDir = Directory.GetCurrentDirectory();
        var configPath = Path.Combine(projectDir, "appsettings.json");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddJsonFile(configPath);
        });
    }
}
