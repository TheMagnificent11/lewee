using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pizzeria.Auth;
using Pizzeria.Common;
using Pizzeria.Configuration;
using Pizzeria.ServiceDefaults;
using Pizzeria.Store.Data;
using Pizzeria.Store.Domain;

var builder = Host.CreateApplicationBuilder(args);

// Add service defaults (required for service discovery)
builder.AddServiceDefaults();

// Get the database connection string via service discovery
var databaseName = ServiceNames.GetPizzaStoreDatabaseName();

// Register database context - connection string will be resolved at runtime via service discovery
builder.Services.AddDbContext<StoreDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString(databaseName)
        ?? throw new InvalidOperationException($"Connection string for '{databaseName}' not found");

    options.UseNpgsql(connectionString);
});

// Register database seeder
builder.Services.AddScoped<Lewee.Infrastructure.Data.IDatabaseSeeder<StoreDbContext>, StoreSeeder>();

// Register configuration services
builder.Services.AddScoped<PizzeriaStoreDatabaseConfigurationService>();
builder.Services.AddScoped<KeycloakConfigurationService>();

// Register Keycloak HTTP client with service discovery
builder.Services.AddHttpClient<KeycloakHttpClient>((serviceProvider, client) =>
{
    // Use service discovery to get the Keycloak URL
    client.BaseAddress = new Uri($"http://{ServiceNames.AuthServer}");
    client.Timeout = TimeSpan.FromMinutes(1);
});

var host = builder.Build();

// Get the logger
var logger = host.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Pizzeria Configuration starting...");

try
{
    using var scope = host.Services.CreateScope();

    // Configure database first
    var dbConfigService = scope.ServiceProvider.GetRequiredService<PizzeriaStoreDatabaseConfigurationService>();
    await dbConfigService.ConfigureAsync(CancellationToken.None);

    // Then configure Keycloak
    var keycloakConfigService = scope.ServiceProvider.GetRequiredService<KeycloakConfigurationService>();
    await keycloakConfigService.ConfigureAsync(CancellationToken.None);

    logger.LogInformation("✅ Pizzeria Configuration completed successfully");
}
catch (Exception ex)
{
    logger.LogError(ex, "❌ Pizzeria Configuration failed: {Message}", ex.Message);
    return 1;
}

return 0;
