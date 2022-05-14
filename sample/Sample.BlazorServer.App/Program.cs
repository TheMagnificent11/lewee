using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Saji.Application;
using Saji.Infrastructure.Data;
using Saji.Infrastructure.Logging;
using Saji.Infrastructure.Settings;
using Sample.BlazorServer.App.Areas.Identity;
using Sample.Customers.Application;
using Sample.Customers.Infrastructure.Data;
using Sample.Identity.Infrastructure.Data;
using Sample.Weather.Application;

var builder = WebApplication.CreateBuilder(args);

var identityConnectionString = builder.Configuration.GetConnectionString("Identity");
var customersConnectionString = builder.Configuration.GetConnectionString("Customers");

var appSettings = builder.Configuration.GetSettings<ApplicationSettings>("ApplicationSettings");
var seqSettings = builder.Configuration.GetSettings<SeqSettings>("SeqSettings");

builder.Host.ConfigureLogging(appSettings, seqSettings);

builder.Services
    .AddApplication(typeof(CreateCustomerCommand).Assembly)
    .AddApplication(typeof(GetWeatherForecastsQuery).Assembly)
    .AddPipelineBehaviors();

builder.Services.ConfigureIdentityDatabase(identityConnectionString);
builder.Services.ConfigureDatabase<CustomerDbContext>(customersConnectionString);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services
    .AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<IdentityDbContext>()
    .AddDbContextCheck<CustomerDbContext>();

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

app.MigrationDatabase<IdentityDbContext>();
app.MigrationDatabase<CustomerDbContext>();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
