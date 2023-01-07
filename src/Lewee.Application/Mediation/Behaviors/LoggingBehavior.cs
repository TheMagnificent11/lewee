using MediatR;
using Serilog.Context;

namespace Lewee.Application.Mediation.Behaviors;

internal class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IApplicationRequest
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using (LogContext.PushProperty("CorrelationId", request.CorrelationId))
#pragma warning disable IDE0030 // Use coalesce expression (both values need to be same type to use coalesce)
        using (LogContext.PushProperty("TenantId", request.TenantId.HasValue ? request.TenantId.Value : "No Tenant"))
#pragma warning restore IDE0030 // Use coalesce expression
        {
            return next();
        }
    }
}
