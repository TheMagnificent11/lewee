using MediatR;
using Serilog.Context;

namespace Sample.Restaurant.Application;

internal class TableLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, ITableRequest, IRequest<TResponse>
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using (LogContext.PushProperty("TableNumber", request.TableNumber))
        {
            return next();
        }
    }
}
