using Correlate;

namespace Lewee.Infrastructure.AspNet.WebApi;

internal static class CorrelationContextAccessorExtensions
{
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
