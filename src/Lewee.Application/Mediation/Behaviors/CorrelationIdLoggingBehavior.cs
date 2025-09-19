using FreeMediator;
using Lewee.Application.Mediation.Requests;
using Lewee.Shared;
using Microsoft.Extensions.Logging;

namespace Lewee.Application.Mediation.Behaviors;

internal class CorrelationIdLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<CorrelationIdLoggingBehavior<TRequest, TResponse>> logger;

    public CorrelationIdLoggingBehavior(ILogger<CorrelationIdLoggingBehavior<TRequest, TResponse>> logger)
    {
        this.logger = logger;
    }

    public Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var correlationId = request is IApplicationRequest applicationRequest
            ? applicationRequest.CorrelationId
            : Guid.NewGuid();

        using (this.logger.BeginScope(new Dictionary<string, object>
        {
            { LoggingConsts.CorrelationId, correlationId }
        }))
        {
            return next();
        }
    }
}
