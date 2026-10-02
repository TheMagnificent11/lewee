namespace Pizzeria.Common;

public static class Endpoints
{
    public static class RouteTokens
    {
        public const string OrderId = "orderId";
        public const string PizzaId = "pizzaId";
    }

    public static class StoreApi
    {
        public const string Pizzas = "/pizzas";
        public const string Orders = "/orders";
        public const string Users = "/users";
        public const string Order = $"/orders/{{{RouteTokens.OrderId}}}";
        public const string Pizza = $"/pizzas/{{{RouteTokens.PizzaId}}}";
    }
}
