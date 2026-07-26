namespace Pizzeria.Store.Contracts;

public static class PageRoutes
{
    public const string Home = "/";
    public const string Error = "/error";
    public const string SignOut = "/signout";
    public const string Orders = "/orders";
    public const string OrderRoutePattern = "/orders/{orderId:guid}";

    public static string GetOrderRoute(Guid orderId) => $"{Orders}/{orderId}";
}
