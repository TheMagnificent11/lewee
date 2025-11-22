namespace Pizzeria.Store.Api.Customers;

internal record CreateCustomerRequest
{
    public string ExternalUserId { get; init; }
}
