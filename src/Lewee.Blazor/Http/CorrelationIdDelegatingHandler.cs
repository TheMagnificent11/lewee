using Correlate;
using Lewee.Shared;

namespace Lewee.Blazor.Http;

/// <summary>
/// Correlation ID Delegating Handler
/// </summary>
public class CorrelationIdDelegatingHandler : DelegatingHandler
{
    private readonly ICorrelationContextAccessor correlationContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="CorrelationIdDelegatingHandler"/> class
    /// </summary>
    /// <param name="correlationContextAccessor">Correlation context accessor</param>
    public CorrelationIdDelegatingHandler(ICorrelationContextAccessor correlationContextAccessor)
    {
        this.correlationContextAccessor = correlationContextAccessor;
    }

    /// <inheritdoc />
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.Headers.Contains(RequestHeaders.CorrelationId))
        {
            return base.SendAsync(request, cancellationToken);
        }

        request.Headers.Add(
            RequestHeaders.CorrelationId,
            this.correlationContextAccessor?.CorrelationContext?.CorrelationId ?? Guid.NewGuid().ToString());

        return base.SendAsync(request, cancellationToken);
    }
}
