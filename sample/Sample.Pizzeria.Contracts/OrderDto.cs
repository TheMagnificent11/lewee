namespace Sample.Pizzeria.Contracts;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
public record OrderDto(Guid Id, DateTime OrderDate, string Status, OrderPizzaDto[] Pizzas);
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
