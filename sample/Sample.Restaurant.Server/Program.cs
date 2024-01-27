using FastEndpoints;
using FastEndpoints.Swagger;
using Lewee.Infrastructure.AspNet.Auth;
using Lewee.Infrastructure.AspNet.Observability;
using Lewee.Infrastructure.AspNet.SignalR;
using Lewee.Infrastructure.Data;
using Lewee.Infrastructure.SqlServer;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Sample.Restaurant.Application;
using Sample.Restaurant.Domain;
using Sample.Restaurant.Server.Configuration;
using Sample.Restaurant.SqlServer;
using Serilog;

namespace Sample.Restaurant.Server;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

        var useSqlServer = builder.Configuration.GetValue<bool>("UseSqlServer");

        var connectionString = useSqlServer
            ? builder.Configuration.GetConnectionString("Sample.Restaurant.SqlServer")
            : builder.Configuration.GetConnectionString("Sample.Restaurant.Postgres");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ApplicationException("Could not find database connection string");
        }

        var logger = builder.Environment.ConfigureLogging(builder.Configuration);
        builder.Host.UseSerilog(logger, dispose: true);

        builder.Services.AddMapper();

        if (useSqlServer)
        {
            builder.Services.AddSqlServerDatabaseWithSeeder<SqlServerRestaurantDbContext, SqlServerRestaurantDbSeeder>(
                connectionString,
                typeof(MenuItem).Assembly);
        }
        else
        {
            //builder.Services.AddPostgreSqlDatabaseWithSeeder<RestaurantDbContext, RestaurantDbSeeder>(
            //    connectionString,
            //    typeof(MenuItem).Assembly);
        }

        builder.Services
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
            .AddHealthChecks();

        if (useSqlServer)
        {
            //builder.Services.AddDbContextCheck<SqlServerRestaurantDbContext>();
        }
        else
        {
            // TODO
        }

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

        if (useSqlServer)
        {
            await app.Services.MigrateDatabase<SqlServerRestaurantDbContext>(seedData: true);
        }
        else
        {
            // TODO
        }

        app.Run();
    }
}
