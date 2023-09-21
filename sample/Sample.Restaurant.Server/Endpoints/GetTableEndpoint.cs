using Lewee.Infrastructure.AspNet.WebApi;
using Sample.Restaurant.Application;

namespace Sample.Restaurant.Server.Endpoints;

public sealed class GetTableEndpoint : QueryEndpoint<TableDetailsDto>
{
    protected override string Route => "/api/v1/tables/{tableNumber}";

    protected override string Name => "GetDetails";

    protected override bool IsAnonymousAllowed => true;

    public override async Task HandleAsync(CancellationToken ct)
    {
        var query = new GetTableDetailsQuery(
            this.CorrelationId,
            this.Route<int>("tableNumber"));
        var result = await this.Mediator.Send(query, ct);

        await this.ToResponse(result, ct);
    }

    protected override void ConfigureErrorRouteHandling(RouteHandlerBuilder builder)
    {
        builder.Produces(404);
    }
}
