using Lewee.Application;
using Lewee.Infrastructure.AspNet.Auth;
using Lewee.Infrastructure.AspNet.Logging;
using Lewee.Infrastructure.AspNet.Settings;
using Lewee.Infrastructure.AspNet.SignalR;
using Lewee.Infrastructure.AspNet.WebApi;
using Lewee.Infrastructure.Data;
using Sample.Restaurant.Application;
using Sample.Restaurant.Infrastructure.Data;
using Sample.Restaurant.Infrastructure.Data.Seeding;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Sample.Restaurant");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new ApplicationException("Could not find database connection string");
}

var appSettings = builder.Configuration.GetSettings<ApplicationSettings>(nameof(ApplicationSettings));
var seqSettings = builder.Configuration.GetSettings<SeqSettings>(nameof(SeqSettings));
var migrateDatabases = builder.Configuration.GetValue<bool>("MigrateDatabases");
/* var serviceBusSettings = builder.Configuration.GetSettings<ServiceBusSettings>(nameof(ServiceBusSettings)); */

builder.Host.ConfigureLogging(appSettings, seqSettings);

builder.Services.AddMapper();

builder.Services.ConfigureCorsDefaultPolicy(builder.Configuration["AllowedOrigins"] ?? string.Empty);

builder.Services
    .ConfigureDatabaseWithSeeder<RestaurantDbContext, RestaurantDbSeeder>(connectionString)
    .ConfigureAuthenticatedUserService()
    .ConfigureRestaurantData() // TODO: ideally this would not be needed if IRepository would be registered globally
#if DEBUG
    .AddDatabaseDeveloperPageExceptionFilter()
#endif
    /* .ConfigureServiceBusPublisher(serviceBusSettings) */
    .AddRestaurantApplication()
    .ConfigureControllers()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .ConfigureSignalR()
    .AddHealthChecks()
    .AddDbContextCheck<RestaurantDbContext>();

var application = builder.Build();

application.UseCors()
    .UseSerilogIngestion()
    .UseHealthChecks("/health")
    .UseHttpsRedirection()
    .UseAuthorization();

application.MapControllers();
application.MapHub<ClientEventHub>("/events");

if (application.Environment.IsDevelopment())
{
    application.UseMigrationsEndPoint();
    application.UseSwagger();
    application.UseSwaggerUI();
}

if (migrateDatabases)
{
    await application.MigrationDatabase<RestaurantDbContext>(seedData: true);
}

application.Run();
