using Lewee.Shared;
using MediatR;
using Serilog;

namespace Lewee.Application.Mediation.Behaviors;

internal class FailureLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
{
    private readonly ILogger logger;

    public FailureLoggingBehavior(ILogger logger)
    {
        this.logger = logger.ForContext<FailureLoggingBehavior<TRequest, TResponse>>();
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var result = await next();

        if (result.IsSuccess)
        {
            return result;
        }

        var statusId = (int)result.Status;

        if (statusId >= 500)
        {
            this.logger.Error("Unexpected error occured {@Errors}", result.Errors);
        }
        else
        {
            this.logger.Warning("Bad request {@Errors}", result.Errors);
        }

        return result;
    }
}
