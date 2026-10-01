namespace Pizzeria.Store.Contracts;

public static class PageRoutes
{
    public const string Home = "/";
    public const string Error = "/error";
    public const string SignOut = "/signout";
    public const string Orders = "/orders";
    public const string OrderRoutePattern = "/orders/{orderId:guid}";
    public const string CheckoutRoutePattern = "/orders/{orderId:guid}/checkout";

    public static string GetOrderRoute(Guid orderId) => $"{Orders}/{orderId}";

    public static string GetCheckoutRoute(Guid orderId) => $"{Orders}/{orderId}/checkout";
}
