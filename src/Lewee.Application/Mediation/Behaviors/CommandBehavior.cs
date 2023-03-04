using Lewee.Application.Mediation.Requests;
using Lewee.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Lewee.Application.Mediation.Behaviors;

internal class CommandBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public CommandBehavior(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request.ClientId != null)
        {
            this.httpContextAccessor.HttpContext.Items[HttpContextConsts.ClientId] = request.ClientId;
        }

        return next();
    }
}
