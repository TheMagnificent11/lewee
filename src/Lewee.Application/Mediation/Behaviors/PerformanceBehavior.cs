using MediatR;
using Microsoft.Extensions.Logging;

namespace Lewee.Application.Mediation.Behaviors;

internal class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PerformanceBehavior{TRequest, TResponse}"/> class
    /// </summary>
    /// <param name="logger">
    /// Logger
    /// </param>
    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
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
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using (this.logger.BeginTimedOperation($"{typeof(TRequest).FullName} Handler"))
        {
            return await next(cancellationToken);
        }
    }
}
