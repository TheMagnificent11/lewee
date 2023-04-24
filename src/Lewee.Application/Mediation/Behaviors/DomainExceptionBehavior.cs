using Lewee.Application.Mediation.Requests;
using Lewee.Domain;
using MediatR;
using Serilog;

namespace Lewee.Application.Mediation.Behaviors;

internal class DomainExceptionBehavior<TCommand, TResponse> : IPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand, IRequest<TResponse>
    where TResponse : CommandResult
{
    private readonly ILogger logger;

    public DomainExceptionBehavior(ILogger logger)
    {
        this.logger = logger.ForContext<DomainExceptionBehavior<TCommand, TResponse>>();
    }

    public Task<TResponse> Handle(TCommand request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return next();
        }
        catch (DomainException ex)
        {
            this.logger.Information(ex, "Domain exception caught");

            var result = CommandResult.Fail(ResultStatus.BadRequest, ex.Message);

            return Task.FromResult((TResponse)result);
        }
    }
}
