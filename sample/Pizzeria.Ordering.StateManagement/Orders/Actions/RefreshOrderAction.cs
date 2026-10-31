namespace Pizzeria.Ordering.StateManagement.Orders.Actions;

public record RefreshOrderAction
{
    public Guid OrderId { get; init; }
}
