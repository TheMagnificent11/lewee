using Microsoft.Extensions.Logging;
using Refit;

namespace Lewee.Blazor.Logging;

/// <summary>
/// Extension methods for <see cref="ApiException"/>
/// </summary>
public static class ApiExceptionExtensions
{
    /// <summary>
    /// Logs the appropriate message based on <see cref="ApiException.StatusCode"/>
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="exception">Exception</param>
    public static void LogApiException(this ILogger logger, ApiException exception)
    {
        switch ((int)exception.StatusCode)
        {
            case > 400 and < 500:
                logger.LogInformation(
                    exception,
                    "Request failed (Status Code: {StatusCode}, Message: {BadRequestMessage})",
                    exception.StatusCode,
                    exception.Message);
                return;

            case >= 500:
                logger.LogError(
                    exception,
                    "Request failed due to server error (Status Code: {StateCode}, Message: {Message})",
                    exception.StatusCode,
                    exception.Message);
                return;
        }
    }
}
