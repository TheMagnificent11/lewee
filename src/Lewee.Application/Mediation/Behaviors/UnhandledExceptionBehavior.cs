using FreeMediator;
using Microsoft.Extensions.Logging;

namespace Lewee.Application.Mediation.Behaviors;

internal class UnhandledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger logger;

    public UnhandledExceptionBehavior(ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> logger)
    {
        this.logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next(cancellationToken);
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;

            this.logger.LogError(
                ex,
                "Request: Unhandled Exception for Request {Name} {@Request}",
                requestName,
                request);

            // TODO: instead of re-throwing, return `Result`
            throw;
        }
    }
}
