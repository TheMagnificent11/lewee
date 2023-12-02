using FastEndpoints;
using FastEndpoints.Swagger;
using Lewee.Infrastructure.AspNet.Auth;
using Lewee.Infrastructure.AspNet.Observability;
using Lewee.Infrastructure.AspNet.SignalR;
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

        var logger = builder.Environment.ConfigureLogging(builder.Configuration);
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
            .AddRestaurantApplication()
            .AddCorrelationIdServices()
            .AddFastEndpoints()
            .SwaggerDocument(x =>
            {
                x.DocumentSettings = y =>
                {
                    y.Title = "Restaurant API";
                    y.Version = "v1";
                };
            })
            .ConfigureSignalR()
            .AddHealthChecks()
            .AddDbContextCheck<RestaurantDbContext>();

        builder.Services.AddRazorPages();

        var app = builder.Build();

        // TODO: https://github.com/TheMagnificent11/lewee/issues/15
        //app.UseSerilogIngestion();

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

        app
            .UseCorrelationIdMiddleware()
            .UseFastEndpoints()
            .UseHealthChecks("/health")
            .UseHttpsRedirection()
            .UseBlazorFrameworkFiles()
            .UseStaticFiles()
            .UseRouting();

        app.MapRazorPages();
        app.MapHub<ClientEventHub>("/events");
        app.MapFallbackToFile("index.html");

        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
            app.UseSwaggerGen();
        }

        await app.Services.MigrateDatabase<RestaurantDbContext>(seedData: true);

        app.Run();
    }
}
