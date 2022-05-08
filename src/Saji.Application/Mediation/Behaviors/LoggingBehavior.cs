using MediatR;
using Serilog.Context;

namespace Saji.Application.Mediation.Behaviors;

/// <summary>
/// Logging Behavior
/// </summary>
/// <typeparam name="TRequest">
/// Request type
/// </typeparam>
/// <typeparam name="TReponse">
/// Response type
/// </typeparam>
public class LoggingBehavior<TRequest, TReponse> : IPipelineBehavior<TRequest, TReponse>
    where TRequest : IRequest<TReponse>, IApplicationRequest
{
    /// <summary>
    /// Handles request and adds stuctural logging properties
    /// </summary>
    /// <param name="request">
    /// Request to validate
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <param name="next">
    /// Next behaviour
    /// </param>
    /// <returns>
    /// Response
    /// </returns>
    public Task<TReponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TReponse> next)
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
