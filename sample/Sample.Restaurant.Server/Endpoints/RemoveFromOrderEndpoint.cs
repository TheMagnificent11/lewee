using FastEndpoints;
using Lewee.Infrastructure.AspNet.WebApi;
using Sample.Restaurant.Application;

namespace Sample.Restaurant.Server.Endpoints;

public sealed class RemoveFromOrderEndpoint : CommandEndpoint<EmptyRequest>
{
    protected override string Route => "/api/v1/tables/{tableNumber}/menu-items/{menuItemId}";

    protected override CommandType CommandType => CommandType.Delete;

    protected override string Name => "RemoveMenuItem";

    protected override bool IsAnonymousAllowed => true;

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var command = new RemoveMenuItemCommand(
            this.CorrelationId,
            this.Route<int>("tableNumber"),
            this.Route<Guid>("menuItemId"));
        var result = await this.Mediator.Send(command, ct);

        await this.ToResponse(result, ct);
    }
}
