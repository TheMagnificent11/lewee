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
        public const string AddPizzaToOrder = $"/orders/{{{RouteTokens.OrderId}}}/pizzas/{{{RouteTokens.PizzaId}}}";
        public const string RemovePizzaFromOrder = $"/orders/{{{RouteTokens.OrderId}}}/pizzas/{{{RouteTokens.PizzaId}}}";
        public const string GetOrder = $"/orders/{{{RouteTokens.OrderId}}}";
        public const string SubmitPickupOrder = $"/orders/{{{RouteTokens.OrderId}}}/submit-pickup";
        public const string SubmitDeliveryOrder = $"/orders/{{{RouteTokens.OrderId}}}/submit-delivery";
        public const string StartMakingOrder = $"/orders/{{{RouteTokens.OrderId}}}/start-making";
        public const string MarkOrderPrepared = $"/orders/{{{RouteTokens.OrderId}}}/prepared";
        public const string MarkOrderPickedUp = $"/orders/{{{RouteTokens.OrderId}}}/picked-up";
        public const string MarkOrderDelivered = $"/orders/{{{RouteTokens.OrderId}}}/delivered";
        public const string Pizza = $"/pizzas/{{{RouteTokens.PizzaId}}}";

        public static string GetAddPizzaToOrderEndpoint(Guid orderId, Guid pizzaId)
        {
            return AddPizzaToOrder
                .Replace($"{{{RouteTokens.OrderId}}}", orderId.ToString(), StringComparison.OrdinalIgnoreCase)
                .Replace($"{{{RouteTokens.PizzaId}}}", pizzaId.ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
