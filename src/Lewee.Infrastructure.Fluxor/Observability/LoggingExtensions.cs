using Correlate;
using Lewee.Common;
using Microsoft.Extensions.Logging;

namespace Lewee.Infrastructure.Fluxor.Observability;

/// <summary>
/// Logging Extensions
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Starts a correlation ID logging scope
    /// </summary>
    /// <param name="logger">Logger</param>
    /// <param name="accessor">Correlation context accessor</param>
    /// <param name="action">Request action</param>
    /// <returns><see cref="IDisposable"/></returns>
    public static IDisposable? BeginCorrelationIdScope(
        this ILogger logger,
        ICorrelationContextAccessor accessor,
        IRequestAction action)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(accessor);
        ArgumentNullException.ThrowIfNull(action);

        accessor.SetCorrelationId(action);

        var loggingProps = new Dictionary<string, object>(StringComparer.Ordinal)
        {
            {
                LoggingConsts.CorrelationId,
                action.CorrelationId
            },
        };

        return logger.BeginScope(loggingProps);
    }
}
