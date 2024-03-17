using FastEndpoints;
using Lewee.Infrastructure.AspNet.WebApi;
using Sample.Pizzeria.Application;
using Sample.Pizzeria.Contracts;

namespace Sample.Pizzeria.Endpoints;

public sealed class StartOrderEndpoint : CommandEndpoint<EmptyRequest>
{
    protected override string Route => OrderRoutes.StartOrder;

    protected override CommandType CommandType => CommandType.Post;

    protected override string Name => "StartOrder";

    protected override bool IsAnonymousAllowed => false;

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var command = new StartOrderCommand(this.UserId!, this.CorrelationId);

        var result = await this.Mediator.Send(command, ct);

        await this.ToResponse(result, ct);
    }
}
