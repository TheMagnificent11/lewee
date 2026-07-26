using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lewee.Infrastructure.Data;

/// <summary>
/// Database Health Check Configuration
/// </summary>
public static class DatabaseHealthConfiguration
{
    /// <summary>
    /// Adds database health check for the specified DbContext
    /// </summary>
    /// <typeparam name="TDbContext">The type of the DbContext to check</typeparam>
    /// <param name="services">Services collection</param>
    /// <returns>The updated services collection</returns>
    public static IServiceCollection AddDatabaseHealthCheck<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHealthChecks()
            .AddDbContextCheck<TDbContext>("database", tags: ["live"]);

        return services;
    }

    /// <summary>
    /// Maps health check endpoints for the application
    /// </summary>
    /// <param name="app">Web application</param>
    /// <returns>The updated web application</returns>
    public static WebApplication UseHealthEndpoints(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        // Override the default /health endpoint to be a liveness check (for Aspire orchestration)
        // This excludes readiness checks and only includes basic service health
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = healthCheck => !healthCheck.Tags.Contains("ready"),
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
