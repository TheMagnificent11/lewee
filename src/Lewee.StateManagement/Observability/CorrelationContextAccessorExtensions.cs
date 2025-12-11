using Correlate;

namespace Lewee.StateManagement.Observability;

/// <summary>
/// Correlation Context Accessor Extension Methods
/// </summary>
public static class CorrelationContextAccessorExtensions
{
    /// <summary>
    /// Sets a new correlation ID on the correlation context
    /// </summary>
    /// <param name="accessor">The correlation context accessor</param>
    /// <param name="action">The request action</param>
    /// <returns>The new correlation ID</returns>
    public static Guid SetNewCorrelationId(
        this ICorrelationContextAccessor accessor,
        IRequestAction action)
    {
        ArgumentNullException.ThrowIfNull(accessor);
        ArgumentNullException.ThrowIfNull(action);

        var newCorrelationId = Guid.NewGuid();

        accessor.CorrelationContext = new CorrelationContext
        {
            CorrelationId = action.CorrelationId.ToString(),
        };

        return newCorrelationId;
    }
}
