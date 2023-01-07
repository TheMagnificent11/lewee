using Lewee.Application.Mediation.Responses;
using MediatR;
using Serilog;

namespace Lewee.Application.Mediation.Behaviors;

internal class CommandFailureLoggingBehavior<TCommand> : IPipelineBehavior<TCommand, CommandResult>
    where TCommand : ICommand
{
    private readonly ILogger logger;

    public CommandFailureLoggingBehavior(ILogger logger)
    {
        this.logger = logger.ForContext<CommandFailureLoggingBehavior<TCommand>>();
    }

    public async Task<CommandResult> Handle(TCommand request, RequestHandlerDelegate<CommandResult> next, CancellationToken cancellationToken)
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
