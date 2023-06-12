using MediatR;
using Serilog;

namespace Lewee.Application.Mediation.Behaviors;

internal class UnhandledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger logger;

    public UnhandledExceptionBehavior(ILogger logger)
    {
        this.logger = logger.ForContext<UnhandledExceptionBehavior<TRequest, TResponse>>();
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;

            this.logger.Error(ex, "Request: Unhandled Exception for Request {Name} {@Request}", requestName, request);

            // TODO: instead of re-throwing, return `Result`
            throw;
        }
    }
}
