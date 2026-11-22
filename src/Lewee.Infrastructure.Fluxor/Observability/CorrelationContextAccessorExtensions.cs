using Correlate;

namespace Lewee.Infrastructure.Fluxor.Observability;

/// <summary>
/// Extension methods for <see cref="ICorrelationContextAccessor"/>
/// </summary>
public static class CorrelationContextAccessorExtensions
{
    /// <summary>
    /// Sets the correlation context on the accessor from the given request action's correlation ID
    /// </summary>
    /// <param name="accessor">Correlation context accessor</param>
    /// <param name="action">Request action</param>
    public static void SetCorrelationId(
        this ICorrelationContextAccessor accessor,
        IRequestAction action)
    {
        ArgumentNullException.ThrowIfNull(accessor);
        ArgumentNullException.ThrowIfNull(action);

        accessor.CorrelationContext = new CorrelationContext
        {
            CorrelationId = action.CorrelationId.ToString(),
        };
    }
}
