using FastEndpoints;
using MediatR;
using Pizzeria.Store.Application.Orders;

using CommonEndpoints = Pizzeria.Common.Endpoints;

namespace Pizzeria.Store.Api.Endpoints;

internal sealed class StartOrderEndpoint : Endpoint<EmptyRequest, EmptyResponse>
{
    private IMediator Mediator => this.HttpContext.RequestServices.GetRequiredService<IMediator>();

    public override void Configure()
    {
        this.Post(CommonEndpoints.StoreApi.Orders);
        this.Description(x => x.WithName("StartOrder").Produces(200));
    }

    public override async Task HandleAsync(EmptyRequest request, CancellationToken ct)
    {
        var correlationId = Guid.NewGuid();
        var result = await this.Mediator.Send(new StartOrderCommand(correlationId), ct);

        if (result.IsSuccess)
        {
            await this.SendOkAsync(ct);
            return;
        }

        foreach (var error in result.Errors)
        {
            this.AddError(error);
        }

        this.ThrowIfAnyErrors();
    }
}
