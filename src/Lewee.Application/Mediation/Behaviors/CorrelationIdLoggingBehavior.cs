using Lewee.Application.Mediation.Requests;
using Lewee.Shared;
using MediatR;
using Serilog.Context;

namespace Lewee.Application.Mediation.Behaviors;

internal class CorrelationIdLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var correlationId = request is IApplicationRequest applicationRequest
            ? applicationRequest.CorrelationId
            : Guid.NewGuid();

        using (LogContext.PushProperty(LoggingConsts.CorrelationId, correlationId))
        {
            return next();
        }
    }
}
