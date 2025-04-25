using FreeMediator;
using Lewee.Application.Mediation.Requests;
using Lewee.Shared;
using Microsoft.Extensions.Logging;

namespace Lewee.Application.Mediation.Behaviors;

internal class TenantLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ITenantRequest
{
    private readonly ILogger<TenantLoggingBehavior<TRequest, TResponse>> logger;

    public TenantLoggingBehavior(ILogger<TenantLoggingBehavior<TRequest, TResponse>> logger)
    {
        this.logger = logger;
    }

    public Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        using (this.logger.BeginScope(new Dictionary<string, object>()
        {
            { LoggingConsts.TenantId, request.TenantId }
        }))
        {
            return next(cancellationToken);
        }
    }
}
