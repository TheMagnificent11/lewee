using FastEndpoints;
using Lewee.Infrastructure.FastEndpoints;
using Pizzeria.Store.Application.Orders;

using CommonEndpoints = Pizzeria.Common.Endpoints;

namespace Pizzeria.Store.Api.Orders;

internal sealed class StartMakingOrderEndpoint : CommandEndpoint<OrderIdRequest>
{
    protected override string Route => CommonEndpoints.StoreApi.StartMakingOrder;

    protected override string Name => "StartMakingOrder";

    protected override CommandType CommandType => CommandType.Post;

    protected override bool IsAnonymousAllowed => false;

    public override async Task HandleAsync(OrderIdRequest request, CancellationToken ct)
    {
        var result = await this.Mediator.Send(new StartMakingOrderCommand(request.OrderId), ct);

        await this.ToResponseAsync(result, ct);
    }
}
