using Lewee.Infrastructure.FastEndpoints;
using Pizzeria.Store.Application.Pizzas;

using CommonEndpoints = Pizzeria.Common.Endpoints;

namespace Pizzeria.Store.Api.Pizzas;

internal sealed class RemovePizzaEndpoint : CommandEndpoint<RemovePizzaRequest>
{
    protected override string Route => CommonEndpoints.StoreApi.Pizza;

    protected override string Name => "RemovePizza";

    protected override CommandType CommandType => CommandType.Delete;

    protected override bool IsAnonymousAllowed => false;

    public override async Task HandleAsync(RemovePizzaRequest request, CancellationToken ct)
    {
        var result = await this.Mediator.Send(new RemovePizzaCommand(request.PizzaId), ct);

        await this.ToResponseAsync(result, ct);
    }
}
