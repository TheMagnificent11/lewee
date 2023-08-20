using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.AspNet.WebApi;

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
    [ExcludeFromCodeCoverage]
    public static IServiceCollection ConfigureControllers(this IServiceCollection services)
    {
        services
            .AddControllers(options =>
            {
                options.Conventions.Add(new KebabCaseControllerModelConvention());
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

        services.AddProblemDetails();

        return services;
    }
}
