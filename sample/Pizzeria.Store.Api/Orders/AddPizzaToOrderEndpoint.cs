using Lewee.Infrastructure.FastEndpoints;
using Pizzeria.Store.Application.Orders;

using CommonEndpoints = Pizzeria.Common.Endpoints;

namespace Pizzeria.Store.Api.Orders;

internal sealed class AddPizzaToOrderEndpoint : CommandEndpoint<AddPizzaToOrderRequest>
{
    protected override string Route => CommonEndpoints.StoreApi.AddPizzaToOrder;

    protected override string Name => "AddPizzaToOrder";

    protected override CommandType CommandType => CommandType.Put;

    protected override bool IsAnonymousAllowed => false;

    public override async Task HandleAsync(AddPizzaToOrderRequest request, CancellationToken ct)
    {
        var result = await this.Mediator.Send(
            new AddPizzaToOrderCommand(request.OrderId, request.PizzaId),
            ct);

        await this.ToResponseAsync(result, ct);
    }
}
