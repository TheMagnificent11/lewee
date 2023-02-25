using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.AspNet.WebApi;

/// <summary>
/// CORS Configuration
/// </summary>
public static class CorsConfiguration
{
    /// <summary>
    /// Configures default CORS policy
    /// </summary>
    /// <param name="services">Services collection</param>
    /// <param name="allowedOrigins">Semicolon-separated list of allowed origins</param>
    public static void ConfigureCorsDefaultPolicy(this IServiceCollection services, string allowedOrigins)
    {
        services.AddCors(options => options.AddDefaultPolicy(policyBuilder =>
        {
            var origins = allowedOrigins
                .Split(';')
                .Distinct()
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            policyBuilder = origins.Any() ? policyBuilder.WithOrigins(origins) : policyBuilder.AllowAnyOrigin();

            policyBuilder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }));
    }
}
