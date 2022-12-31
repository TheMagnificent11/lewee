using MediatR;
using Serilog;

namespace Saji.Application.Mediation.Behaviors;

/// <summary>
/// Performance Behavior
/// </summary>
/// <typeparam name="TRequest">
/// Request type
/// </typeparam>
/// <typeparam name="TResponse">
/// Response type
/// </typeparam>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PerformanceBehavior{TRequest, TResponse}"/> class
    /// </summary>
    /// <param name="logger">
    /// Logger
    /// </param>
    public PerformanceBehavior(ILogger logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// Handles request and adds logging for timing
    /// </summary>
    /// <param name="request">
    /// Request
    /// </param>
    /// <param name="next">
    /// Next behavior
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token
    /// </param>
    /// <returns>
    /// Asynchronous task contain a <typeparamref name="TResponse"/>
    /// </returns>
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using (this.logger.BeginTimedOperation($"{typeof(TRequest).FullName} Handler"))
        {
            return next();
        }
    }
}
