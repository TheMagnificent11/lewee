using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.Controllers;

/// <summary>
/// Controller Configuration
/// </summary>
public static class ControllerConfiguration
{
    /// <summary>
    /// Configures the ASP.Net conrollers
    /// </summary>
    /// <param name="services">Services collection</param>
    /// <returns>Updated services collection</returns>
    public static IServiceCollection ConfigureControllers(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Conventions.Add(new KebabCaseControllerModelConvention());
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        return services;
    }
}
