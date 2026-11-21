using FastEndpoints;
using Lewee.Common;
using Lewee.Infrastructure.FastEndpoints;
using Microsoft.EntityFrameworkCore;
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

    public override async Task HandleAsync(UpdateOrderRequest req, CancellationToken ct)
    {
        var orderId = this.Route<Guid>(Endpoints.RouteTokens.OrderId);

        var actionCount = (req.SubmitAsDelivery ? 1 : 0)
            + (req.SubmitAsPickup ? 1 : 0)
            + (req.FulfillmentAction.HasValue ? 1 : 0);
        if (actionCount != 1)
        {
            await this.ToResponseAsync(
                CommandResult.Fail(ResultStatus.BadRequest, "Exactly one update action must be specified."),
                ct);
            return;
        }

        CommandResult result;
        try
        {
            if (req.SubmitAsDelivery)
            {
                result = await this.Mediator.Send(
                    new SubmitDeliveryOrderCommand(orderId, req.DeliveryAddress ?? string.Empty),
                    ct);
            }
            else if (req.FulfillmentAction.HasValue)
            {
                result = await this.Mediator.Send(
                    new UpdateOrderStatusCommand(orderId, req.FulfillmentAction.Value),
                    ct);
            }
            else
            {
                result = await this.Mediator.Send(new SubmitPickupOrderCommand(orderId), ct);
            }

            await this.ToResponseAsync(result, ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            this.HttpContext.Response.StatusCode = StatusCodes.Status409Conflict;
            await this.HttpContext.Response.WriteAsync("The order changed. Refresh and retry.", ct);
        }
    }
}
