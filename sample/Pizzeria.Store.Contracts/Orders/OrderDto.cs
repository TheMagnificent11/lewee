namespace Pizzeria.Store.Contracts.Orders;

public record OrderDto
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public DateTime StartedDateTime { get; init; }
    public DateTime? SubmittedDateTime { get; init; }
    public DateTime? PreparedDateTime { get; init; }
    public DateTime? CompletedDateTime { get; init; }
    public string? DeliveryAddress { get; init; }
    public IReadOnlyList<OrderPizzaDto> Pizzas { get; init; } = [];
    public decimal TotalCost { get; init; }
}
