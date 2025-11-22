using System.Diagnostics.CodeAnalysis;
using Pizzeria.Store.Contracts;
using Refit;

namespace Pizzeria.Store.Web.Services;

[SuppressMessage("Performance", "CA1515:Consider making public types internal", Justification = "API client interface should be public for Refit and DI")]
public interface IPizzeriaApiClient
{
    [Get("/pizzas")]
    Task<PizzaDto[]> GetPizzasAsync(CancellationToken cancellationToken = default);

    [Post("/orders")]
    Task StartOrderAsync(CancellationToken cancellationToken = default);

    [Put("/orders/{orderId}/pizzas/{pizzaId}")]
    Task AddPizzaToOrderAsync(Guid orderId, Guid pizzaId, CancellationToken cancellationToken = default);

    [Post("/customers")]
    Task CreateCustomerAsync([Body] CreateCustomerRequest request, CancellationToken cancellationToken = default);
}
