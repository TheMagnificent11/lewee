namespace Pizzeria.Store.Api.Customers;

public record CreateCustomerRequest
{
    public string ExternalId { get; init; }
}
