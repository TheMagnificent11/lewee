using Lewee.Infrastructure.FastEndpoints;
using MediatR;
using Pizzeria.Store.Application.Customers;

using CommonEndpoints = Pizzeria.Common.Endpoints;

namespace Pizzeria.Store.Api.Endpoints;

internal sealed class CreateCustomerEndpoint : CommandEndpoint<CreateCustomerRequest>
{
    protected override string Route => CommonEndpoints.StoreApi.Customers;

    protected override string Name => "CreateCustomer";

    protected override CommandType CommandType => CommandType.Post;

    protected override bool IsAnonymousAllowed => false;

    public override async Task HandleAsync(CreateCustomerRequest request, CancellationToken ct)
    {
        var result = await this.Mediator.Send(
            new CreateCustomerCommand(request.ExternalUserId, this.CorrelationId),
            ct);

        await this.ToResponseAsync(result, ct);
    }
}
