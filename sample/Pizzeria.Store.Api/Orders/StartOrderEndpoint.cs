using Lewee.Infrastructure.FastEndpoints;
using Pizzeria.Store.Application.Orders;

using CommonEndpoints = Pizzeria.Common.Endpoints;

namespace Pizzeria.Store.Api.Orders;

internal sealed class StartOrderEndpoint : CommandEndpoint<EmptyRequest>
{
    protected override string Route => CommonEndpoints.StoreApi.Orders;

    protected override string Name => "StartOrder";

    protected override CommandType CommandType => CommandType.Post;

    protected override bool IsAnonymousAllowed => false;

    public override async Task HandleAsync(EmptyRequest request, CancellationToken ct)
    {
        var result = await this.Mediator.Send(new StartOrderCommand(this.CorrelationId), ct);
        await this.ToResponseAsync(result, ct);
    }
}
