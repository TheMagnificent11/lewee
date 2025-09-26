using FastEndpoints;
using Lewee.Infrastructure.AspNet.WebApi;
using Pizzeria.Common;
using Pizzeria.Store.Application.Orders;

namespace Pizzeria.Store.Api.Orders;

public sealed class AddPizzaToOrderEndpoint : CommandEndpoint<EmptyRequest>
{
    protected override string Route => Endpoints.StoreApi.AddPizzaToOrder; 

    protected override CommandType CommandType => CommandType.Put;

    protected override string Name => "Add Pizza To Order";

    protected override bool IsAnonymousAllowed => true;

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var orderId = this.Route<Guid>(Endpoints.RouteTokens.OrderId);
        var pizzaId = this.Route<Guid>(Endpoints.RouteTokens.PizzaId);

        var command = new AddPizzaToOrderCommand(orderId, pizzaId, this.CorrelationId);
        var result = await this.Mediator.Send(command, ct);

        await this.ToResponse(result, ct);
    }
}
