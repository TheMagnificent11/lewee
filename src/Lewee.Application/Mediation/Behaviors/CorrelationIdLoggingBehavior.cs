using Correlate;
using Lewee.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lewee.Application.Mediation.Behaviors;

internal class CorrelationIdLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICorrelationContextAccessor correlationContextAccessor;
    private readonly ILogger<CorrelationIdLoggingBehavior<TRequest, TResponse>> logger;

    public CorrelationIdLoggingBehavior(
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<CorrelationIdLoggingBehavior<TRequest, TResponse>> logger)
    {
        this.correlationContextAccessor = correlationContextAccessor;
        this.logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var correlationId = this.correlationContextAccessor.GetCorrelationId();

        using (this.logger.BeginScope(new Dictionary<string, object>(StringComparer.Ordinal)
        {
            { LoggingConsts.CorrelationId, correlationId }
        }))
        {
            return await next(cancellationToken);
        }
    }
}
