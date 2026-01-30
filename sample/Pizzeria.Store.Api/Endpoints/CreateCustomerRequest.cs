namespace Pizzeria.Store.Api.Endpoints;

internal sealed record CreateCustomerRequest
{
    public string ExternalUserId { get; init; } = string.Empty;
}
