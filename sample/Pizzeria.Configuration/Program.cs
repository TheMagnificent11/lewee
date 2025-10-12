using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pizzeria.Configuration.Services;
using Pizzeria.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpClient();
builder.Services.AddTransient<IKeycloakConfigurationService, KeycloakConfigurationService>();
builder.Services.AddTransient<IPizzeriaStoreDatabaseConfigurationService, PizzeriaStoreDatabaseConfigurationService>();

var host = builder.Build();

var keycloakService = host.Services.GetRequiredService<IKeycloakConfigurationService>();
await keycloakService.ConfigureAsync();

var databaseService = host.Services.GetRequiredService<IPizzeriaStoreDatabaseConfigurationService>();
await databaseService.ConfigureAsync();

Console.WriteLine("Configuration completed successfully");
