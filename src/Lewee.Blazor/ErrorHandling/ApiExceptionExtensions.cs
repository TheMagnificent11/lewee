using Microsoft.Extensions.Logging;

namespace Lewee.Blazor.ErrorHandling;

/// <summary>
/// Extension methods for <see cref="ApiException"/>
/// </summary>
public static class ApiExceptionExtensions
{
    /// <summary>
    /// Logs the appropriate message based on <see cref="ApiException.StatusCode"/>
    /// </summary>
    /// <param name="exception">Exception</param>
    /// <param name="logger">Logger</param>
    public static void Log(this ApiException exception, ILogger logger)
    {
        if (exception.StatusCode < 400)
        {
            logger.LogWarning(exception, "Unexpected response status (Status Code: {StatusCode})", exception.StatusCode);
            return;
        }

        if (exception.StatusCode == 400)
        {
            logger.LogInformation(
                exception,
                "Request failed (Status Code: {StatusCode}, Message: {BadRequestMessage}, Response Body: {BadRequestResponseBody})",
                exception.StatusCode,
                exception.Message,
                exception.Response);
            return;
        }

        if (exception.StatusCode > 400 && exception.StatusCode < 500)
        {
            logger.LogInformation(
                exception,
                "Request failed (Status Code: {StatusCode}, Message: {ErrorMessage}",
                exception.StatusCode,
                exception.Message);
            return;
        }

        logger.LogError(
            exception,
            "Request failed due to server error (Status Code: {StateCode}, Message: {Message})",
            exception.StatusCode,
            exception.Message);
    }
}
