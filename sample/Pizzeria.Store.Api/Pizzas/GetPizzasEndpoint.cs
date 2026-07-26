using Lewee.Infrastructure.FastEndpoints;
using Pizzeria.Store.Application.Pizzas;
using Pizzeria.Store.Contracts.Pizzas;

using CommonEndpoints = Pizzeria.Common.Endpoints;

namespace Pizzeria.Store.Api.Pizzas;

internal sealed class GetPizzasEndpoint : QueryEndpoint<IEnumerable<PizzaDto>>
{
    protected override string Route => CommonEndpoints.StoreApi.Pizzas;

    protected override string Name => "GetPizzas";

    protected override bool IsAnonymousAllowed => false;

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await this.Mediator.Send(new GetPizzasQuery(), ct);
        await this.ToResponseAsync(result, ct);
    }
}
