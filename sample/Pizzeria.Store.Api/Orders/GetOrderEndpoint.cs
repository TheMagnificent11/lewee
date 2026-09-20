using FastEndpoints;
using Lewee.Infrastructure.FastEndpoints;
using Pizzeria.Common;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Contracts.Orders;

using CommonEndpoints = Pizzeria.Common.Endpoints;

namespace Pizzeria.Store.Api.Orders;

internal sealed class GetOrderEndpoint : QueryEndpoint<OrderDto>
{
    protected override string Route => CommonEndpoints.StoreApi.GetOrder;

    protected override string Name => "GetOrder";

    protected override bool IsAnonymousAllowed => false;

    public override async Task HandleAsync(CancellationToken ct)
    {
        var orderId = this.Route<Guid>(Endpoints.RouteTokens.OrderId);

        var result = await this.Mediator.Send(new GetOrderQuery(orderId), ct);

        await this.ToResponseAsync(result, ct);
    }
}
