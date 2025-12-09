using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Pizzeria.Store.Data;

namespace Pizzeria.Store.Web.Infrastructure;

internal static class DatabaseHealthConfiguration
{
    public static IServiceCollection AddDatabaseHealthCheck(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDbContextCheck<StoreDbContext>("database", tags: ["live"]);

        return services;
    }

    public static WebApplication UseHealthEndpoints(this WebApplication app)
    {
        // Override the default /health endpoint to be a liveness check (for Aspire orchestration)
        // This excludes readiness checks and only includes basic service health
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = healthCheck => !healthCheck.Tags.Contains("ready"),
        });

        // Map the /ready endpoint for startup readiness checks (for consuming applications)
        app.MapHealthChecks("/ready", new HealthCheckOptions
        {
            Predicate = healthCheck => healthCheck.Tags.Contains("ready"),
        });

        // Map other default endpoints (but skip /health since we mapped it above)
        if (app.Environment.IsDevelopment())
        {
            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks("/alive", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live"),
            });
        }

        return app;
    }
}
