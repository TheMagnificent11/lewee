using Lewee.Application.Mediation.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Lewee.Infrastructure.Controllers;

/// <summary>
/// Query Result Extensions
/// </summary>
public static class QueryResultExtensions
{
    /// <summary>
    /// Converts to an <see cref="IActionResult"/> object
    /// </summary>
    /// <typeparam name="T">Operation result data type</typeparam>
    /// <param name="result">Operation result to convert</param>
    /// <returns>Action result</returns>
    public static IActionResult ToActionResult<T>(this QueryResult<T> result)
        where T : class
    {
        return result.Status switch
        {
            ResultStatus.Success => new OkObjectResult(result.Data),

            ResultStatus.NotFound => new NotFoundResult(),

            ResultStatus.Unauthenticated => new UnauthorizedResult(),

            ResultStatus.Unauthorized => new ForbidResult(),

            _ => new StatusCodeResult((int)result.Status),
        };
    }
}
