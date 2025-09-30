namespace Lewee.Blazor.Tests.Integration;

public record PizzaOrder
{
    public Guid Id { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string PizzaType { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public DateTime CreatedAt { get; init; }
}
