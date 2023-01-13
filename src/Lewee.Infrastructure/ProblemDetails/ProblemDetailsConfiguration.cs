using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Infrastructure.ProblemDetails;

/// <summary>
/// Problem Details Configuration
/// </summary>
public static class ProblemDetailsConfiguration
{
    /// <summary>
    /// Configures problem details
    /// </summary>
    /// <param name="services">Services collection</param>
    public static void ConfigureProblemDetails(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Instance = context.HttpContext.Request.Path,
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = "Please refer to the errors property for additional details"
                };

                return new BadRequestObjectResult(problemDetails)
                {
                    ContentTypes = { "application/json" }
                };
            };
        });
    }
}
