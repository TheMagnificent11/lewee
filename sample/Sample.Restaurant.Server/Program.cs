using FastEndpoints;
using Lewee.Infrastructure.AspNet.Auth;
using Lewee.Infrastructure.AspNet.Observability;
using Lewee.Infrastructure.AspNet.SignalR;
using Lewee.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Sample.Restaurant.Application;
using Sample.Restaurant.Domain;
using Sample.Restaurant.Infrastructure.Data;
using Sample.Restaurant.Server.Configuration;

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

        builder.Services.AddMapper();

        builder.Services
            .AddDbContextFactory<RestaurantDbContext>(options => options.UseSqlServer(connectionString))
            .AddLeweeDatabaseConfigurationWithSeeder<RestaurantDbContext, RestaurantDbSeeder>(typeof(MenuItem).Assembly)
            .ConfigureAuthenticatedUserService()
#if DEBUG
            .AddDatabaseDeveloperPageExceptionFilter()
#endif
            .AddRestaurantApplication()
            .AddCorrelationIdServices()
            .AddFastEndpoints()
            .AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Restaurant API",
                    Version = "v1"
                });
            })
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

        app
            .UseCorrelationIdMiddleware()
            .UseFastEndpoints()
            .UseHealthChecks("/health")
            .UseHttpsRedirection()
            .UseBlazorFrameworkFiles()
            .UseStaticFiles()
            .UseRouting();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API V1");
            });
        }

        app.MapRazorPages();
        app.MapHub<ClientEventHub>("/events");
        app.MapFallbackToFile("index.html");

        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();

            // TODO: fix swagger
            //app.UseSwaggerGen();
        }

        await app.Services.MigrateDatabaseAsync<RestaurantDbContext>(seedData: true);

        app.Run();
    }
}
