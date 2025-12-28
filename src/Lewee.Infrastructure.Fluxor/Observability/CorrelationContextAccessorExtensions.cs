using Correlate;

namespace Lewee.Infrastructure.Fluxor.Observability;

internal static class CorrelationContextAccessorExtensions
{
    public static void SetNewCorrelationId(
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
