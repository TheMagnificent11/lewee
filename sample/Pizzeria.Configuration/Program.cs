using Lewee.Auth.Infrastructure.Data;
using Lewee.Infrastructure.Data;
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

using CommonEnvironments = Pizzeria.Common.Environments;

var builder = Host.CreateApplicationBuilder(args);

// Add service defaults (required for service discovery)
builder.AddServiceDefaults();

// Get the database connection string via service discovery
var databaseName = ServiceNames.PizzaStoreDatabaseName;

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

builder.Services.AddDbContext<AuthDbContext>((serviceProvider, options) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString(databaseName);

    if (!string.IsNullOrWhiteSpace(connectionString))
    {
        options.UseNpgsql(connectionString);
    }
});

builder.Services.AddKeycloakAdminClient(
    CommonEnvironments.Auth.RealmName,
    client => client.BaseAddress = new Uri($"https+http://{ServiceNames.AuthServer}"));

// Register database seeder
builder.Services.AddTransient<IDatabaseSeeder<StoreDbContext>, StoreSeeder>();
builder.Services.AddTransient<IDatabaseSeeder<AuthDbContext>, AuthSeeder>();

// Register configuration service
builder.Services.AddTransient<PizzeriaConfigurationService>();

using var host = builder.Build();

await host.StartAsync();

var configurationService = host.Services.GetRequiredService<PizzeriaConfigurationService>();
var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

try
{
    logger.LogInformation("Pizzeria Configuration starting...");

    await configurationService.ConfigureAsync(lifetime.ApplicationStopping);

    logger.LogInformation("Pizzeria Configuration completed successfully");
}
finally
{
    await host.StopAsync();
}
