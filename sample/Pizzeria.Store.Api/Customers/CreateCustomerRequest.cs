namespace Pizzeria.Store.Api.Customers;

public record CreateCustomerRequest
{
    public string Username { get; init; }
    public string Password { get; init; }
}
