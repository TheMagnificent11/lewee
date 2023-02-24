using Lewee.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Lewee.Infrastructure.AspNet.WebApi;

/// <summary>
/// Command Result Extensions
/// </summary>
public static class CommandResultExtensions
{
    /// <summary>
    /// Converts to a <see cref="ValidationProblemDetails"/> object
    /// </summary>
    /// <param name="result">Command result to convert</param>
    /// <returns>Validation problem details</returns>
    public static ValidationProblemDetails ToProblemDetails(this CommandResult result)
    {
        var problemDetails = new ValidationProblemDetails()
        {
            Status = (int)ResultStatus.BadRequest
        };

        if (problemDetails.Errors != null)
        {
            result.Errors
               .ToList()
               .ForEach(i => problemDetails.Errors.Add(i.Key, i.Value.ToArray()));
        }

        return problemDetails;
    }

    /// <summary>
    /// Converts to an <see cref="IActionResult"/> object
    /// </summary>
    /// <param name="result">Operation result to convert</param>
    /// <returns>Action result</returns>
    public static IActionResult ToActionResult(this CommandResult result)
    {
        switch (result.Status)
        {
            case ResultStatus.Success:
                return new OkResult();

            case ResultStatus.NotFound:
                return new NotFoundResult();

            case ResultStatus.BadRequest:
                var problems = result.ToProblemDetails();
                return new BadRequestObjectResult(problems);

            default:
                return new StatusCodeResult((int)result.Status);
        }
    }
}
