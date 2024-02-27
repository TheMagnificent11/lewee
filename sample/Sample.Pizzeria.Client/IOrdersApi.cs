using Refit;
using Sample.Pizzeria.Contracts;

namespace Sample.Pizzeria.Client;

public interface IOrdersApi
{
    [Get(OrderRoutes.GetUserOrders)]
    Task<OrderDto[]> GetUserOrders(CancellationToken cancellationToken);
}
