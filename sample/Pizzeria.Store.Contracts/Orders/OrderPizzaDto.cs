namespace Pizzeria.Store.Contracts.Orders;

public record OrderPizzaDto
{
    public Guid Id { get; init; }
    public Guid PizzaId { get; init; }
    public string PizzaName { get; init; } = string.Empty;
    public decimal PizzaPrice { get; init; }
    public int Quantity { get; init; }
    public decimal LineTotal { get; init; }
}
