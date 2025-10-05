using Lewee.Application.Mediation.Requests;
using Lewee.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Lewee.Application.Mediation.Behaviors;

internal class DomainExceptionBehavior<TCommand, TResponse> : IPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand, IRequest<TResponse>
    where TResponse : CommandResult
{
    private readonly ILogger logger;

    public DomainExceptionBehavior(ILogger<DomainExceptionBehavior<TCommand, TResponse>> logger)
    {
        this.logger = logger;
    }

    public async Task<TResponse> Handle(
        TCommand request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next(cancellationToken);
        }
        catch (DomainException ex)
        {
            this.logger.LogDomainExceptionCaught(ex);

            var result = CommandResult.Fail(ResultStatus.BadRequest, ex.Message);

            return (TResponse)result;
        }
    }
}
