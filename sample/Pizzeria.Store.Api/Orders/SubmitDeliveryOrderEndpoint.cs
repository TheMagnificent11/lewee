using FastEndpoints;
using Lewee.Infrastructure.FastEndpoints;
using Pizzeria.Store.Application.Orders;

using CommonEndpoints = Pizzeria.Common.Endpoints;

namespace Pizzeria.Store.Api.Orders;

internal sealed class SubmitDeliveryOrderEndpoint : CommandEndpoint<SubmitOrderRequest>
{
    protected override string Route => CommonEndpoints.StoreApi.SubmitDeliveryOrder;

    protected override string Name => "SubmitDeliveryOrder";

    protected override CommandType CommandType => CommandType.Post;

    protected override bool IsAnonymousAllowed => false;

    public override async Task HandleAsync(SubmitOrderRequest request, CancellationToken ct)
    {
        var result = await this.Mediator.Send(
            new SubmitDeliveryOrderCommand(request.OrderId, request.DeliveryAddress ?? string.Empty),
            ct);

        await this.ToResponseAsync(result, ct);
    }
}
