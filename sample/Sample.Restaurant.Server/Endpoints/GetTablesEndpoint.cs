using Lewee.Infrastructure.AspNet.WebApi;
using Sample.Restaurant.Application;

namespace Sample.Restaurant.Server.Endpoints;

public sealed class GetTablesEndpoint : QueryEndpoint<IEnumerable<TableDto>>
{
    protected override string Route => "/api/v1/tables";

    protected override string Name => "GetAll";

    protected override bool IsAnonymousAllowed => true;

    public override async Task HandleAsync(CancellationToken ct)
    {
        var query = new GetTablesQuery(this.CorrelationId);
        var result = await this.Mediator.Send(query, ct);

        await this.SendAsync(result.Data!, 200, ct);
    }
}
