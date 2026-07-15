using Pizzeria.Store.Contracts.Pizzas;
using Pizzeria.Store.Contracts.Users;
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

    [Post("/customers")]
    Task CreateCustomerAsync(
        [Body] CreateCustomerRequest request,
        CancellationToken cancellationToken = default);
}
