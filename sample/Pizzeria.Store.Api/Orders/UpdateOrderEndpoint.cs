using FastEndpoints;
using Lewee.Common;
using Lewee.Infrastructure.FastEndpoints;
using Pizzeria.Common;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Contracts.Orders;

using CommonEndpoints = Pizzeria.Common.Endpoints;

namespace Pizzeria.Store.Api.Orders;

/// <summary>
/// Handles all order mutations (adding/removing a pizza and submitting the order) via a single
/// PUT endpoint, driven by which field is populated on <see cref="UpdateOrderRequest"/>.
/// </summary>
internal sealed class UpdateOrderEndpoint : CommandEndpoint<UpdateOrderRequest>
{
    protected override string Route => CommonEndpoints.StoreApi.Order;

    protected override string Name => "UpdateOrder";

    protected override CommandType CommandType => CommandType.Put;

    protected override bool IsAnonymousAllowed => false;

    public override async Task HandleAsync(UpdateOrderRequest request, CancellationToken ct)
    {
        var orderId = this.Route<Guid>(Endpoints.RouteTokens.OrderId);

        var result = await this.SendCommandAsync(orderId, request, ct);

        await this.ToResponseAsync(result, ct);
    }

    private async Task<CommandResult> SendCommandAsync(Guid orderId, UpdateOrderRequest request, CancellationToken ct)
    {
        if (request.AddPizzaId.HasValue)
        {
            return await this.Mediator.Send(new AddPizzaToOrderCommand(orderId, request.AddPizzaId.Value), ct);
        }

        if (request.RemovePizzaId.HasValue)
        {
            return await this.Mediator.Send(new RemovePizzaFromOrderCommand(orderId, request.RemovePizzaId.Value), ct);
        }

        if (request.SubmitAsDelivery)
        {
            return await this.Mediator.Send(
                new SubmitDeliveryOrderCommand(orderId, request.DeliveryAddress ?? string.Empty),
                ct);
        }

        if (request.SubmitAsPickup)
        {
            return await this.Mediator.Send(new SubmitPickupOrderCommand(orderId), ct);
        }

        return CommandResult.Fail(ResultStatus.BadRequest, "No update action specified.");
    }
}
