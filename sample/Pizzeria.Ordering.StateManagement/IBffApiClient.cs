using Lewee.Auth.Contracts;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.Contracts.Pizzas;
using Refit;

namespace Pizzeria.Ordering.StateManagement;

public interface IBffApiClient
{
    [Get("pizzas")]
    Task<IEnumerable<PizzaDto>> GetPizzasAsync(CancellationToken cancellationToken = default);

    [Post("orders")]
    Task StartOrderAsync(CancellationToken cancellationToken = default);

    [Get("orders/{orderId}")]
    Task<OrderDto> GetOrderAsync(Guid orderId, CancellationToken cancellationToken = default);

    [Put("orders/{orderId}")]
    Task UpdateOrderAsync(
        Guid orderId,
        [Body] UpdateOrderRequest request,
        CancellationToken cancellationToken = default);

    [Put("orders/{orderId}/pizzas/{pizzaId}")]
    Task SetOrderPizzaQuantityAsync(
        Guid orderId,
        Guid pizzaId,
        [Body] SetPizzaQuantityRequest request,
        CancellationToken cancellationToken = default);

    [Delete("orders/{orderId}/pizzas/{pizzaId}")]
    Task RemovePizzaFromOrderAsync(
        Guid orderId,
        Guid pizzaId,
        CancellationToken cancellationToken = default);

    [Post("users")]
    Task CreateUserAsync(
        [Body] CreateUserRequest request,
        CancellationToken cancellationToken = default);
}
