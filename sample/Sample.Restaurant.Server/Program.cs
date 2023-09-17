using System.Text.Json;
using Lewee.Infrastructure.AspNet.Auth;
using Lewee.Infrastructure.AspNet.Observability;
using Lewee.Infrastructure.AspNet.SignalR;
using Lewee.Infrastructure.AspNet.WebApi;
using Lewee.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Sample.Restaurant.Application;
using Sample.Restaurant.Domain;
using Sample.Restaurant.Infrastructure.Data;
using Sample.Restaurant.Server.Configuration;
using Serilog;

namespace Sample.Restaurant.Server;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

        var connectionString = builder.Configuration.GetConnectionString("Sample.Restaurant");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ApplicationException("Could not find database connection string");
        }

        var logger = builder.Host.ConfigureLogging(builder.Configuration, builder.Environment);
        builder.Host.UseSerilog(logger, dispose: true);

        builder.Services.AddMapper();

        builder.Services
            .ConfigureDatabaseWithSeeder<RestaurantDbContext, RestaurantDbSeeder>(
                connectionString,
                typeof(MenuItem).Assembly)
            .ConfigureAuthenticatedUserService()
#if DEBUG
            .AddDatabaseDeveloperPageExceptionFilter()
#endif
            .AddRestaurantApplication();

        builder.Services.AddCorrelationIdServices();

        builder.Services
            .AddControllersWithViews(options =>
            {
                options.Conventions.Add(new KebabCaseControllerModelConvention());
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

        builder.Services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .ConfigureSignalR()
            .AddHealthChecks()
            .AddDbContextCheck<RestaurantDbContext>();

        builder.Services.AddRazorPages();

        var app = builder.Build();

        app.UseResponseCompression();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseCorrelationIdMiddleware();

        app.UseHealthChecks("/health")
            .UseHttpsRedirection()
            .UseBlazorFrameworkFiles()
            .UseStaticFiles()
            .UseRouting();

        app.MapRazorPages();
        app.MapControllers();
        app.MapHub<ClientEventHub>("/events");
        app.MapFallbackToFile("index.html");

        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        await app.MigrationDatabase<RestaurantDbContext>(seedData: true);

        app.Run();
    }
}
