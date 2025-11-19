using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Pizzeria.Auth;
using Pizzeria.Common;
using Pizzeria.Configuration;
using Pizzeria.ServiceDefaults;
using Pizzeria.Store.Data;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (required for service discovery)
builder.AddServiceDefaults();

// Get the database connection string via service discovery
var databaseName = ServiceNames.GetPizzaStoreDatabaseName();

// Register database context - connection string will be resolved at runtime via service discovery
builder.Services.AddDbContext<StoreDbContext>((serviceProvider, options) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString(databaseName);

    if (!string.IsNullOrWhiteSpace(connectionString))
    {
        options.UseNpgsql(connectionString);
    }
});

// Register database seeder
builder.Services.AddTransient<IDatabaseSeeder<StoreDbContext>, StoreSeeder>();

// Register configuration services
builder.Services.AddSingleton<ConfigurationStatusService>();
builder.Services.AddTransient<PizzeriaStoreDatabaseConfigurationService>();
builder.Services.AddHostedService<ConfigurationBackgroundService>();

// Register auth server admin client with service discovery
builder.Services.AddAuthServerAdminClient((serviceProvider, client) =>
{
    // Use service discovery to get the Keycloak URL
    client.BaseAddress = new Uri($"http://{ServiceNames.AuthServer}");
    client.Timeout = TimeSpan.FromMinutes(1);
});

// Add health checks
builder.Services
    .AddHealthChecks()
    .AddCheck<ConfigurationHealthCheck>("configuration_status");

var app = builder.Build();

// Map health check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/alive");

await app.RunAsync();
