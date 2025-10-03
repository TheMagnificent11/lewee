using Lewee.Application.Mediation.Requests;
using Lewee.Shared;
using MediatR;
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

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        using (this.logger.BeginScope(new Dictionary<string, object>(StringComparer.Ordinal)
        {
            { LoggingConsts.TenantId, request.TenantId }
        }))
        {
            return await next(cancellationToken);
        }
    }
}
