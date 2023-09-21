using FastEndpoints;
using Lewee.Infrastructure.AspNet.WebApi;
using Sample.Restaurant.Application;

namespace Sample.Restaurant.Server.Endpoints;

public sealed class AddToOrderEndpoint : CommandEndpoint<EmptyRequest>
{
    protected override string Route => "/api/v1/tables/{tableNumber}/menu-items/{menuItemId}";

    protected override CommandType CommandType => CommandType.Put;

    protected override string Name => "OrderMenuItem";

    protected override bool IsAnonymousAllowed => true;

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var command = new AddMenuItemCommand(
            this.CorrelationId,
            this.Route<int>("tableNumber"),
            this.Route<Guid>("menuItemId"));
        var result = await this.Mediator.Send(command, ct);

        await this.ToResponse(result, ct);
    }

    protected override void ConfigureErrorRouteHandling(RouteHandlerBuilder builder)
    {
        builder
            .ProducesProblemDetails()
            .Produces(404);
    }
}
