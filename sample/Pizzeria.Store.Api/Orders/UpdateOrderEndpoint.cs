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

    public override async Task HandleAsync(UpdateOrderRequest request, CancellationToken ct)
    {
        var orderId = this.Route<Guid>(Endpoints.RouteTokens.OrderId);

        var actionCount = (request.SubmitAsDelivery ? 1 : 0)
            + (request.SubmitAsPickup ? 1 : 0)
            + (request.FulfillmentAction.HasValue ? 1 : 0);
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
            if (request.SubmitAsDelivery)
            {
                result = await this.Mediator.Send(
                    new SubmitDeliveryOrderCommand(orderId, request.DeliveryAddress ?? string.Empty),
                    ct);
            }
            else if (request.FulfillmentAction.HasValue)
            {
                result = await this.Mediator.Send(
                    new UpdateOrderStatusCommand(orderId, request.FulfillmentAction.Value),
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
