using Correlate;
using Lewee.Application.Mediation.Requests;
using Lewee.Shared;
using MediatR;
using Serilog.Context;

namespace Lewee.Application.Mediation.Behaviors;

internal class CorrelationIdLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly string headerCorrelationId;

    public CorrelationIdLoggingBehavior(ICorrelationContextAccessor correlationContextAccessor)
    {
        this.headerCorrelationId =
            correlationContextAccessor?.CorrelationContext?.CorrelationId ?? Guid.NewGuid().ToString();
    }

    public Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var correlationId = this.headerCorrelationId;

        if (request is IApplicationRequest applicationRequest && applicationRequest.CorrelationId.HasValue)
        {
            correlationId = applicationRequest.CorrelationId.ToString();
        }

        using (LogContext.PushProperty(LoggingConsts.CorrelationId, correlationId))
        {
            return next();
        }
    }
}
