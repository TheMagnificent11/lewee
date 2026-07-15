using Lewee.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pizzeria.Common;
using Pizzeria.Configuration;
using Pizzeria.ServiceDefaults;
using Pizzeria.Store.Data;

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

// Register database seeder
builder.Services.AddTransient<IDatabaseSeeder<StoreDbContext>, StoreSeeder>();

// Register configuration service
builder.Services.AddTransient<PizzeriaStoreDatabaseConfigurationService>();

using var host = builder.Build();

var dbConfigService = host.Services.GetRequiredService<PizzeriaStoreDatabaseConfigurationService>();
var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Pizzeria Configuration starting...");

await dbConfigService.ConfigureAsync(lifetime.ApplicationStopping);

logger.LogInformation("Pizzeria Configuration completed successfully");
