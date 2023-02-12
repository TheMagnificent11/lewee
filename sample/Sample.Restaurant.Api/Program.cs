using Lewee.Application;
using Lewee.Infrastructure.Auth;
using Lewee.Infrastructure.Data;
using Lewee.Infrastructure.Logging;
using Lewee.Infrastructure.Settings;
using Sample.Restaurant.Application;
using Sample.Restaurant.Infrastructure.Data;

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

builder.Services
    .ConfigureDatabase<RestaurantDbContext>(connectionString)
    .ConfigureAuthenticatedUserService()
    .ConfigureRestaurantData() // TODO: ideally this would not be needed if IRepository would be registered globally
    .AddRestaurantApplication()
    /* .ConfigureServiceBusPublisher(serviceBusSettings) */
    .AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllers();

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<RestaurantDbContext>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

if (migrateDatabases)
{
    app.MigrationDatabase<RestaurantDbContext>();
}

app.Run();
