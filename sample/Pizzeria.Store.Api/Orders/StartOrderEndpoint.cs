using FastEndpoints;
using Lewee.Infrastructure.AspNet.WebApi;
using Pizzeria.Common;
using Pizzeria.Store.Application.Orders;

namespace Pizzeria.Store.Api.Orders;

public sealed class StartOrderEndpoint : CommandEndpoint<EmptyRequest>
{
    protected override string Route => Endpoints.StoreApi.Orders;

    protected override CommandType CommandType => CommandType.Post;

    protected override string Name => "Start Order";

    protected override bool IsAnonymousAllowed => true;

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var command = new StartOrderCommand(Guid.NewGuid().ToString(), this.CorrelationId);
        var result = await this.Mediator.Send(command, ct);

        await this.ToResponse(result, ct);
    }
}
