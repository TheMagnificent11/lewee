using MediatR;
using Serilog.Context;

namespace Lewee.Application.Mediation.Behaviors;

/// <summary>
/// Logging Behavior
/// </summary>
/// <typeparam name="TRequest">
/// Request type
/// </typeparam>
/// <typeparam name="TResponse">
/// Response type
/// </typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IApplicationRequest
{
    /// <summary>
    /// Handles request and adds stuctural logging properties
    /// </summary>
    /// <param name="request">
    /// Request to log
    /// </param>
    /// <param name="next">
    /// Next behavior
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <returns>
    /// Asynchronous task contain a <typeparamref name="TResponse"/>
    /// </returns>
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
