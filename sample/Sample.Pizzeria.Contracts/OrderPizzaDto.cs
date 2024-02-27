namespace Sample.Pizzeria.Contracts;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
public record OrderPizzaDto(Guid Id, string Name, decimal Price, int Quantity)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
{
    public decimal Total => this.Price * this.Quantity;
}
