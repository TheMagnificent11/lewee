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
        public const string AddPizzaToOrder = $"/orders/{{{RouteTokens.OrderId}}}/pizzas/{{{RouteTokens.PizzaId}}}";

        public static string GetAddPizzaToOrderEndpoint(Guid orderId, Guid pizzaId)
        {
            return AddPizzaToOrder
                .Replace($"{{{RouteTokens.OrderId}}}", orderId.ToString())
                .Replace($"{{{RouteTokens.PizzaId}}}", pizzaId.ToString());
        }
    }
}
