namespace Pizzeria.Store.Api.Customers;

public record CreateCustomerRequest
{
    public string ExternalUserId { get; init; }
}
