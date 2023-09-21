using FastEndpoints;
using Lewee.Infrastructure.AspNet.WebApi;
using Sample.Restaurant.Application;

namespace Sample.Restaurant.Server.Endpoints;

public sealed class UseTableEndpoint : CommandEndpoint<EmptyRequest>
{
    protected override string Route => "/api/v1/tables/{tableNumber}";

    protected override CommandType CommandType => CommandType.Put;

    protected override string Name => "Use";

    protected override bool IsAnonymousAllowed => true;

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var command = new UseTableCommand(
            this.CorrelationId,
            this.Route<int>("tableNumber"));
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
