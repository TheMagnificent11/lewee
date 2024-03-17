namespace Sample.Pizzeria.Contracts;

public static class OrderRoutes
{
    public const string GetUserOrders = "/api/v1/orders";
    public const string StartOrder = "/api/v1/orders";
    public const string AddPizzaToOrder = "/api/v1/orders/{orderId}/pizzas/{pizzaId}";
    public const string RemovePizzaFromOrder = "/api/v1/orders/{orderId}/pizzas/{pizzaId}";
}
