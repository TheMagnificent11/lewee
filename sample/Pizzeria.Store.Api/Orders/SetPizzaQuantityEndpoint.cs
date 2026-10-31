using FastEndpoints;
using Lewee.Common;
using Lewee.Infrastructure.FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Pizzeria.Common;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Contracts.Orders;

using CommonEndpoints = Pizzeria.Common.Endpoints;

namespace Pizzeria.Store.Api.Orders;

internal sealed class SetPizzaQuantityEndpoint : CommandEndpoint<SetPizzaQuantityRequest>
{
    protected override string Route => CommonEndpoints.StoreApi.OrderPizza;

    protected override string Name => "SetOrderPizzaQuantity";

    protected override CommandType CommandType => CommandType.Put;

    protected override bool IsAnonymousAllowed => false;

    public override async Task HandleAsync(SetPizzaQuantityRequest request, CancellationToken ct)
    {
        if (request.Quantity <= 0)
        {
            await this.ToResponseAsync(
                CommandResult.Fail(ResultStatus.BadRequest, "Pizza quantity must be greater than zero."),
                ct);
            return;
        }

        var command = new SetPizzaQuantityInOrderCommand(
            this.Route<Guid>(Endpoints.RouteTokens.OrderId),
            this.Route<Guid>(Endpoints.RouteTokens.PizzaId),
            request.Quantity);

        try
        {
            await this.ToResponseAsync(await this.Mediator.Send(command, ct), ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            this.HttpContext.Response.StatusCode = StatusCodes.Status409Conflict;
            await this.HttpContext.Response.WriteAsync("The order changed. Refresh and retry.", ct);
        }
    }
}
