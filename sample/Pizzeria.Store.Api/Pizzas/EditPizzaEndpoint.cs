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

    public override async Task HandleAsync(EditPizzaRequest request, CancellationToken ct)
    {
        var result = await this.Mediator.Send(
            new EditPizzaCommand(request.PizzaId, request.Name, request.Description, request.Price),
            ct);

        await this.ToResponseAsync(result, ct);
    }
}
