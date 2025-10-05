using MediatR;
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

            using (this.logger.BeginScope(new Dictionary<string, object>(StringComparer.Ordinal)
            {
                { "RequestName", requestName },
                { "Request", request! }
            }))
            {
                this.logger.LogUnhandledException(
                    ex,
                    requestName,
                    request!);
            }

            // TODO: instead of re-throwing, return `Result`
            throw;
        }
    }
}
