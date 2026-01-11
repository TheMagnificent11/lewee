using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.Refit;

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
        ArgumentNullException.ThrowIfNull(exception);
        ArgumentNullException.ThrowIfNull(logger);

        if (exception.StatusCode < 400)
        {
            logger.LogUnexpectedResponseStatus(exception, exception.StatusCode);
            return;
        }

        if (exception.StatusCode == 400)
        {
            logger.LogBadRequestFailed(
                exception,
                exception.StatusCode,
                exception.Message,
                exception.Response);
            return;
        }

        if (exception.StatusCode > 400 && exception.StatusCode < 500)
        {
            logger.LogClientErrorFailed(
                exception,
                exception.StatusCode,
                exception.Message);
            return;
        }

        logger.LogServerErrorFailed(
            exception,
            exception.StatusCode,
            exception.Message);
    }
}
