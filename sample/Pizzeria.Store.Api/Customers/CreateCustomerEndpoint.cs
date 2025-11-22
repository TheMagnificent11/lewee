using Lewee.Infrastructure.AspNet.WebApi;
using Pizzeria.Common;
using Pizzeria.Store.Application.Customers;

namespace Pizzeria.Store.Api.Customers;

internal sealed class CreateCustomerEndpoint : CommandEndpoint<CreateCustomerRequest>
{
    protected override string Route => Endpoints.StoreApi.Customers;

    protected override CommandType CommandType => CommandType.Post;

    protected override string Name => "Create Customer";

    protected override bool IsAnonymousAllowed => true;

    public override async Task HandleAsync(CreateCustomerRequest req, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(req);

        var command = new CreateCustomerCommand(req.ExternalUserId, this.CorrelationId);
        var result = await this.Mediator.Send(command, ct);

        await this.ToResponseAsync(result, ct);
    }
}
