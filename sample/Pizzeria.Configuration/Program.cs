using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pizzeria.Auth;
using Pizzeria.Common;
using Pizzeria.Configuration;
using Pizzeria.ServiceDefaults;
using Pizzeria.Store.Data;
using Pizzeria.Store.Domain;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddSingleton<ConfigurationStatusService>();
builder.Services.AddScoped<PizzeriaStoreDatabaseConfigurationService>();
builder.Services.AddScoped<KeycloakConfigurationService>();
builder.Services.AddHostedService<ConfigurationBackgroundService>();

// Register Keycloak HTTP client with service discovery
builder.Services.AddHttpClient<KeycloakHttpClient>((serviceProvider, client) =>
{
    // Use service discovery to get the Keycloak URL
    client.BaseAddress = new Uri($"http://{ServiceNames.AuthServer}");
    client.Timeout = TimeSpan.FromMinutes(1);
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck<ConfigurationHealthCheck>("configuration_status");

var app = builder.Build();

// Map health check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/alive");

await app.RunAsync();
