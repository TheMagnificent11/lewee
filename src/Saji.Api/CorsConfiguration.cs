namespace Saji.Api;

/// <summary>
/// CORS Configuration
/// </summary>
public static class CorsConfiguration
{
    /// <summary>
    /// Configures CORS policy
    /// </summary>
    /// <param name="services">
    /// Services collection
    /// </param>
    /// <param name="policyName">
    /// Policy name
    /// </param>
    /// <param name="allowedOrigins">
    /// Semicolon-separated list of allowed origins
    /// </param>
    public static void ConfigureCorsPolicy(this IServiceCollection services, string policyName, string allowedOrigins)
    {
        if (allowedOrigins is null)
        {
            throw new ArgumentNullException(nameof(allowedOrigins));
        }

        services.AddCors(o => o.AddPolicy(policyName, policyBuilder =>
        {
            var origins = allowedOrigins
                .Split(';')
                .Distinct()
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray();

            policyBuilder = origins.Any() ? policyBuilder.WithOrigins(origins) : policyBuilder.AllowAnyOrigin();

            policyBuilder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }));
    }
}
