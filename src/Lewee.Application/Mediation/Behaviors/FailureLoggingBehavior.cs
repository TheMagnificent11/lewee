using Lewee.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lewee.Application.Mediation.Behaviors;

internal class FailureLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly ILogger logger;

    public FailureLoggingBehavior(ILogger<FailureLoggingBehavior<TRequest, TResponse>> logger)
    {
        this.logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var result = await next(cancellationToken);

        if (result.IsSuccess)
        {
            return result;
        }

        var statusId = (int)result.Status;

        if (statusId >= 500)
        {
            this.logger.LogUnexpectedError(result.Errors);
        }
        else
        {
            this.logger.LogBadRequest(result.Errors);
        }

        return result;
    }
}
