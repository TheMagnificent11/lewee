using Lewee.Infrastructure.FastEndpoints;
using Pizzeria.Store.Application.Pizzas;

using CommonEndpoints = Pizzeria.Common.Endpoints;

namespace Pizzeria.Store.Api.Pizzas;

internal sealed class AddPizzaEndpoint : CommandEndpoint<AddPizzaRequest>
{
    protected override string Route => CommonEndpoints.StoreApi.Pizzas;

    protected override string Name => "AddPizza";

    protected override CommandType CommandType => CommandType.Post;

    protected override bool IsAnonymousAllowed => false;

    public override async Task HandleAsync(AddPizzaRequest request, CancellationToken ct)
    {
        var result = await this.Mediator.Send(
            new AddPizzaCommand(request.Name, request.Description, request.Price),
            ct);

        await this.ToResponseAsync(result, ct);
    }
}
