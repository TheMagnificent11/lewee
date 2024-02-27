using Lewee.Infrastructure.AspNet.WebApi;
using Sample.Pizzeria.Application;
using Sample.Pizzeria.Contracts;

namespace Sample.Pizzeria.Endpoints;

public sealed class GetUserOrdersEndpoint : QueryEndpoint<OrderDto[]>
{
    protected override string Route => OrderRoutes.GetUserOrders;

    protected override string Name => "GetOrdersForAuthenticatedUser";

    protected override bool IsAnonymousAllowed => false;

    public override async Task HandleAsync(CancellationToken ct)
    {
        var query = new GetUserOrdersQuery(this.UserId!, this.CorrelationId);
        var result = await this.Mediator.Send(query, ct);

        await this.ToResponse(result, ct);
    }
}
