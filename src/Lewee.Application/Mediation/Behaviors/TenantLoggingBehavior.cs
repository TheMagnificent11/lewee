using Lewee.Application.Mediation.Requests;
using MediatR;
using Serilog.Context;

namespace Lewee.Application.Mediation.Behaviors;

internal class TenantLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ITenantRequest
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using (LogContext.PushProperty(LoggingConsts.TenantId, request.TenantId))
        {
            return next();
        }
    }
}
