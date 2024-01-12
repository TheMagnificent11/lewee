using Lewee.Shared;
using Microsoft.Extensions.Logging;

namespace Lewee.Blazor.Logging;

/// <summary>
/// Logging Extensions
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Starts a correlation ID logging scope
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="correlationId">Correlation ID</param>
    /// <returns><see cref="IDisposable"/></returns>
    public static IDisposable? BeginCorrelationIdScope(this ILogger logger, Guid correlationId)
    {
        var loggingProps = new Dictionary<string, object>
        {
            {
                LoggingConsts.CorrelationId,
                correlationId
            }
        };

        return logger.BeginScope(loggingProps);
    }
}
