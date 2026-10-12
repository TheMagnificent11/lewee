using Lewee.Auth.Api;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.Contracts.Pizzas;
using Refit;

namespace Pizzeria.Ordering.StateManagement;

public interface IBffApiClient
{
    [Get("/pizzas")]
    Task<IEnumerable<PizzaDto>> GetPizzasAsync(CancellationToken cancellationToken = default);

    [Post("/orders")]
    Task StartOrderAsync(CancellationToken cancellationToken = default);

    [Put("/orders/{orderId}")]
    Task UpdateOrderAsync(
        Guid orderId,
        [Body] UpdateOrderRequest request,
        CancellationToken cancellationToken = default);

    [Post("/users")]
    Task CreateUserAsync(
        [Body] CreateUserRequest request,
        CancellationToken cancellationToken = default);
}
