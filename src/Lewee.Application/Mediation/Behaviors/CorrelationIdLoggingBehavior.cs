using Lewee.Shared;
using MediatR;
using Serilog.Context;

namespace Lewee.Application.Mediation.Behaviors;

internal class CorrelationIdLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IApplicationRequest
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using (LogContext.PushProperty(LoggingConsts.CorrelationId, request.CorrelationId))
        {
            return next();
        }
    }
}
