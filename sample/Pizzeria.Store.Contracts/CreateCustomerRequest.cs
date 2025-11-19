namespace Pizzeria.Store.Contracts;

public record CreateCustomerRequest
{
    public string ExternalUserId { get; init; }
}
