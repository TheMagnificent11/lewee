using Lewee.Application.Mediation.Responses;
using Lewee.Domain;
using MediatR;
using Serilog;

namespace Lewee.Application.Mediation.Behaviors;

internal class DomainExceptionBehavior<TCommand> : IPipelineBehavior<TCommand, CommandResult>
    where TCommand : ICommand
{
    private readonly ILogger logger;

    public DomainExceptionBehavior(ILogger logger)
    {
        this.logger = logger.ForContext<DomainExceptionBehavior<TCommand>>();
    }

    public Task<CommandResult> Handle(TCommand request, RequestHandlerDelegate<CommandResult> next, CancellationToken cancellationToken)
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

            return Task.FromResult(CommandResult.Fail(ResultStatus.BadRequest, ex.Message));
        }
    }
}
