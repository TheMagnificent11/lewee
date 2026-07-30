using Lewee.Infrastructure.FastEndpoints;
using Pizzeria.Store.Application.Customers;
using Pizzeria.Store.Contracts.Users;

using CommonEndpoints = Pizzeria.Common.Endpoints;

namespace Pizzeria.Store.Api.Customers;

internal sealed class CreateCustomerEndpoint : CommandEndpoint<CreateCustomerRequest>
{
    protected override string Route => CommonEndpoints.StoreApi.Customers;

    protected override string Name => "CreateCustomer";

    protected override CommandType CommandType => CommandType.Post;

    protected override bool IsAnonymousAllowed => true;

    public override async Task HandleAsync(CreateCustomerRequest request, CancellationToken ct)
    {
        var result = await this.Mediator.Send(
            new CreateCustomerCommand(request.ExternalUserId),
            ct);

        await this.ToResponseAsync(result, ct);
    }
}
