using Fluxor;
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
    .ConfigureDatabase<IRestaurantDbContext, RestaurantDbContext>(connectionString)
    .ConfigureAuthenticatedUserService()
    .AddRestaurantApplication()
    /* .ConfigureServiceBusPublisher(serviceBusSettings) */
    .AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<RestaurantDbContext>();

builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(typeof(Program).Assembly);

#if DEBUG
    options.UseReduxDevTools();
#endif
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

if (migrateDatabases)
{
    app.MigrationDatabase<RestaurantDbContext>();
}

app.Run();
