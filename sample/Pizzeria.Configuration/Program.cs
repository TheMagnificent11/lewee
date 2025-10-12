using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pizzeria.Auth;
using Pizzeria.Common;
using Pizzeria.Configuration.Services;
using Pizzeria.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpClient<KeycloakHttpClient>((serviceProvider, httpClient) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString(ServiceNames.AuthServer);

    if (!string.IsNullOrEmpty(connectionString))
    {
        var baseUrl = connectionString.StartsWith("Endpoint=", StringComparison.OrdinalIgnoreCase)
            ? connectionString["Endpoint=".Length..]
            : connectionString;

        httpClient.BaseAddress = new Uri(baseUrl.TrimEnd('/'));
        httpClient.Timeout = TimeSpan.FromSeconds(5);
    }
});

builder.Services.AddTransient<IAuthServerConfiguration, KeycloakConfigurationService>();
builder.Services.AddTransient<IDatabaseConfigurationService, PizzeriaStoreDatabaseConfigurationService>();

var host = builder.Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInformation("Starting configuration services...");

    var authServerService = host.Services.GetRequiredService<IAuthServerConfiguration>();
    await authServerService.ConfigureAsync();

    var databaseService = host.Services.GetRequiredService<IDatabaseConfigurationService>();
    await databaseService.ConfigureAsync();

    logger.LogInformation("Configuration completed successfully");
}
catch (Exception ex)
{
    logger.LogError(ex, "Configuration failed");
    throw;
}
