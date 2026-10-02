using FastEndpoints;
using Lewee.Infrastructure.FastEndpoints;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Contracts.Orders;

using CommonEndpoints = Pizzeria.Common.Endpoints;

namespace Pizzeria.Store.Api.Orders;

internal sealed class GetOrdersEndpoint : QueryEndpoint<IEnumerable<OrderDto>>
{
    protected override string Route => CommonEndpoints.StoreApi.Orders;

    protected override string Name => "GetOrders";

    protected override bool IsAnonymousAllowed => false;

    public override async Task HandleAsync(CancellationToken ct)
    {
        var filter = this.Query<OrderListFilter?>("filter", isRequired: false) ?? OrderListFilter.Current;

        var result = await this.Mediator.Send(new GetOrdersQuery(filter), ct);

        await this.ToResponseAsync(result, ct);
    }
}
