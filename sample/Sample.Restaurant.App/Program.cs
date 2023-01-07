using Lewee.Application;
using Lewee.Infrastructure.Auth;
using Lewee.Infrastructure.Data;
using Lewee.Infrastructure.Logging;
using Lewee.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Sample.Restaurant.Application;
using Sample.Restaurant.Application.Tables;
using Sample.Restaurant.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

var identityConnectionString = builder.Configuration.GetConnectionString("database");
if (string.IsNullOrWhiteSpace(identityConnectionString))
{
    throw new ApplicationException("Could not find Identity database connection string");
}

var connectionString = builder.Configuration.GetConnectionString("database");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new ApplicationException("Could not find database connection string");
}

var appSettings = builder.Configuration.GetSettings<ApplicationSettings>("ApplicationSettings");
var seqSettings = builder.Configuration.GetSettings<SeqSettings>("SeqSettings");

builder.Host.ConfigureLogging(appSettings, seqSettings);

builder.Services.AddMapper();

builder.Services
    .ConfigureDatabase<IRestaurantDbContext, RestaurantDbContext>(connectionString)
    .AddApplication(typeof(GetTablesQuery).Assembly)
    .AddPipelineBehaviors()
    .ConfigureAuthenticatedUserService()
    .AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<RestaurantDbContext>();

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

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
