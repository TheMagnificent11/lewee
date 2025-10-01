using Pizzeria.Store.Contracts;
using Refit;

namespace Pizzeria.Store.WebClient.Services;

public interface IPizzeriaApiClient
{
    [Get("/pizzas")]
    Task<PizzaDto[]> GetPizzasAsync(CancellationToken cancellationToken = default);

    [Post("/orders")]
    Task StartOrderAsync(CancellationToken cancellationToken = default);

    [Put("/orders/{orderId}/pizzas/{pizzaId}")]
    Task AddPizzaToOrderAsync(Guid orderId, Guid pizzaId, CancellationToken cancellationToken = default);
}