using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Serilog.Context;

namespace Saji.Application.Mediation;

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
    where TRequest : IRequest
{
    /// <summary>
    /// Handles request adds stuctural logging properties
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
    public override async Task<TReponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TReponse> next)
    {
        using (LogContext.PushProperty("CorrelationId", request.CorrelationId))
        using (LogContext.PushProperty("TenantId", request.TenantId.HasValue ? request.TenantId.Value : "No Tenant"))
        {
            return await next();
        }
    }
}
