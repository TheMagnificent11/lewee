using FastEndpoints;
using Lewee.Infrastructure.FastEndpoints;
using Pizzeria.Store.Application.Orders;

using CommonEndpoints = Pizzeria.Common.Endpoints;

namespace Pizzeria.Store.Api.Orders;

internal sealed class MarkOrderDeliveredEndpoint : CommandEndpoint<OrderIdRequest>
{
    protected override string Route => CommonEndpoints.StoreApi.MarkOrderDelivered;

    protected override string Name => "MarkOrderDelivered";

    protected override CommandType CommandType => CommandType.Post;

    protected override bool IsAnonymousAllowed => false;

    public override async Task HandleAsync(OrderIdRequest request, CancellationToken ct)
    {
        var result = await this.Mediator.Send(new MarkOrderPizzasDeliveredCommand(request.OrderId), ct);

        await this.ToResponseAsync(result, ct);
    }
}
