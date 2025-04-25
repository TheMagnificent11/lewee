using FreeMediator;
using Microsoft.Extensions.Logging;

namespace Sample.Restaurant.Application;

internal class TableLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, ITableRequest, IRequest<TResponse>
{
    private readonly ILogger<TableLoggingBehavior<TRequest, TResponse>> logger;

    public TableLoggingBehavior(ILogger<TableLoggingBehavior<TRequest, TResponse>> logger)
    {
        this.logger = logger;
    }

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using (this.logger.BeginScope(new Dictionary<string, object>
        {
            { "TableNumber", request.TableNumber }
        }))
        {
            return next(cancellationToken);
        }
    }
}
