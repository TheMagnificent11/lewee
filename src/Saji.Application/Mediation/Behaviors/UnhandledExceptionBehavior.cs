using MediatR;
using Serilog;

namespace Saji.Application.Mediation.Behaviors;

/// <summary>
/// Unhandled Exception Behaviour
/// </summary>
/// <typeparam name="TRequest">Request type</typeparam>
/// <typeparam name="TResponse">Response type</typeparam>
public class UnhandledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnhandledExceptionBehavior{TRequest, TResponse}"/> class
    /// </summary>
    /// <param name="logger">Logger</param>
    public UnhandledExceptionBehavior(ILogger logger)
    {
        this.logger = logger.ForContext<UnhandledExceptionBehavior<TRequest, TResponse>>();
    }

    /// <summary>
    /// Handle the request
    /// </summary>
    /// <param name="request">Request to handle</param>
    /// <param name="next">Next handler</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An asynchronous task containing a <typeparamref name="TResponse"/></returns>
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;

            this.logger.Error(ex, "Request: Unhandled Exception for Request {Name} {@Request}", requestName, request);

            throw;
        }
    }
}
