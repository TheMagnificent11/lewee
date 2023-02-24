using Lewee.Domain;
using Lewee.Shared;
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
#pragma warning disable Serilog004 // Constant MessageTemplate verifier
            this.logger.Warning(ex, ex.Message);
#pragma warning restore Serilog004 // Constant MessageTemplate verifier

            var result = CommandResult.Fail(ResultStatus.BadRequest, ex.Message);

            return Task.FromResult((TResponse)result);
        }
    }
}
