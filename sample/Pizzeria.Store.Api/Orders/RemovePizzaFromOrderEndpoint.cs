using FastEndpoints;
using Lewee.Infrastructure.FastEndpoints;
using Pizzeria.Store.Application.Orders;

using CommonEndpoints = Pizzeria.Common.Endpoints;

namespace Pizzeria.Store.Api.Orders;

internal sealed class RemovePizzaFromOrderEndpoint : CommandEndpoint<AddPizzaToOrderRequest>
{
    protected override string Route => CommonEndpoints.StoreApi.RemovePizzaFromOrder;

    protected override string Name => "RemovePizzaFromOrder";

    protected override CommandType CommandType => CommandType.Delete;

    protected override bool IsAnonymousAllowed => false;

    public override async Task HandleAsync(AddPizzaToOrderRequest request, CancellationToken ct)
    {
        var result = await this.Mediator.Send(
            new RemovePizzaFromOrderCommand(request.OrderId, request.PizzaId),
            ct);

        await this.ToResponseAsync(result, ct);
    }
}
