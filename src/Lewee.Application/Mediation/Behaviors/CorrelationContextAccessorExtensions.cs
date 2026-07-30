using Correlate;

namespace Lewee.Application.Mediation.Behaviors;

/// <summary>
/// Extension methods for <see cref="ICorrelationContextAccessor"/>
/// </summary>
public static class CorrelationContextAccessorExtensions
{
    /// <summary>
    /// Gets the correlation ID from the correlation context, or generates a new one if not available
    /// </summary>
    /// <param name="correlationContextAccessor">Correlation context accessor</param>
    /// <returns>The current correlation ID, or a new <see cref="Guid"/> if unavailable</returns>
    public static Guid GetCorrelationId(this ICorrelationContextAccessor correlationContextAccessor)
    {
        var cid = correlationContextAccessor?.CorrelationContext?.CorrelationId;

        if (cid == null || !Guid.TryParse(cid, out var correlationId))
        {
            return Guid.NewGuid();
        }

        return correlationId;
    }
}
