using Lewee.Auth.Api;
using Pizzeria.Store.Contracts.Pizzas;
using Refit;

namespace Pizzeria.Store.StateManagement;

public interface IStoreApiClient
{
    [Get("/pizzas")]
    Task<IEnumerable<PizzaDto>> GetPizzasAsync(CancellationToken cancellationToken = default);

    [Post("/orders")]
    Task StartOrderAsync(CancellationToken cancellationToken = default);

    [Put("/orders/{orderId}/pizzas/{pizzaId}")]
    Task AddPizzaToOrderAsync(
        Guid orderId,
        Guid pizzaId,
        CancellationToken cancellationToken = default);

    [Post("/users")]
    Task CreateUserAsync(
        [Body] CreateUserRequest request,
        CancellationToken cancellationToken = default);
}
