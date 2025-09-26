using Lewee.Infrastructure.AspNet.WebApi;
using Pizzeria.Common;
using Pizzeria.Store.Application.Pizzas;
using Pizzeria.Store.Contracts;

namespace Pizzeria.Store.Api.Pizzas;

public sealed class GetPizzasEndpoint : QueryEndpoint<PizzaDto[]>
{
    protected override string Route => Endpoints.StoreApi.Pizzas;

    protected override string Name => "Menu";

    protected override bool IsAnonymousAllowed => true;

    public override async Task HandleAsync(CancellationToken ct)
    {
        var query = new GetPizzasQuery(this.CorrelationId);
        var result = await this.Mediator.Send(query, ct);

        await this.ToResponse(result, ct);
    }
}
