using FastEndpoints;
using Lewee.Common;
using Lewee.Infrastructure.FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Pizzeria.Common;
using Pizzeria.Store.Application.Orders;

using CommonEndpoints = Pizzeria.Common.Endpoints;

namespace Pizzeria.Store.Api.Orders;

internal sealed class RemovePizzaFromOrderEndpoint : CommandEndpoint<EmptyRequest>
{
    protected override string Route => CommonEndpoints.StoreApi.OrderPizza;

    protected override string Name => "RemovePizzaFromOrder";

    protected override CommandType CommandType => CommandType.Delete;

    protected override bool IsAnonymousAllowed => false;

    public override async Task HandleAsync(EmptyRequest request, CancellationToken ct)
    {
        var command = new SetPizzaQuantityInOrderCommand(
            this.Route<Guid>(Endpoints.RouteTokens.OrderId),
            this.Route<Guid>(Endpoints.RouteTokens.PizzaId),
            0);

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
