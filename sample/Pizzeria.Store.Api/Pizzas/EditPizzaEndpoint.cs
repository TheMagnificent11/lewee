using Lewee.Infrastructure.FastEndpoints;
using Pizzeria.Store.Application.Pizzas;

using CommonEndpoints = Pizzeria.Common.Endpoints;

namespace Pizzeria.Store.Api.Pizzas;

internal sealed class EditPizzaEndpoint : CommandEndpoint<EditPizzaRequest>
{
    protected override string Route => CommonEndpoints.StoreApi.Pizza;

    protected override string Name => "EditPizza";

    protected override CommandType CommandType => CommandType.Put;

    protected override bool IsAnonymousAllowed => false;

    public override async Task HandleAsync(EditPizzaRequest req, CancellationToken ct)
    {
        var result = await this.Mediator.Send(
            new EditPizzaCommand(
                this.Route<Guid>(CommonEndpoints.RouteTokens.PizzaId),
                req.Name,
                req.Description,
                req.Price),
            ct);

        await this.ToResponseAsync(result, ct);
    }
}
