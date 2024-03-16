using FastEndpoints;
using Lewee.Infrastructure.AspNet.WebApi;
using Sample.Pizzeria.Application;

namespace Sample.Pizzeria.Endpoints;

public sealed class AddPizzaToOrderEndpoint : CommandEndpoint<EmptyRequest>
{
    protected override string Route => "orders/{orderId}/pizzas/{pizzaId}";

    protected override CommandType CommandType => CommandType.Post;

    protected override string Name => "AddPizzaToOrder";

    protected override bool IsAnonymousAllowed => false;

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var command = new AddPizzaToOrderCommand(
            this.Route<Guid>("orderId"),
            this.Route<int>("pizzaId"),
            this.CorrelationId);

        var result = await this.Mediator.Send(command, ct);

        await this.ToResponse(result, ct);
    }
}
