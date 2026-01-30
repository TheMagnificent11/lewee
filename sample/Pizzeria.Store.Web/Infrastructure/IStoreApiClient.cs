using Pizzeria.Common;
using Pizzeria.Store.Contracts.Pizzas;
using Refit;

namespace Pizzeria.Store.Infrastructure;

internal interface IStoreApiClient
{
    [Get(Endpoints.StoreApi.Pizzas)]
    Task<IEnumerable<PizzaDto>> GetPizzasAsync(CancellationToken cancellationToken = default);

    [Post(Endpoints.StoreApi.Orders)]
    Task StartOrderAsync(CancellationToken cancellationToken = default);

    [Put(Endpoints.StoreApi.AddPizzaToOrder)]
    Task AddPizzaToOrderAsync(Guid orderId, Guid pizzaId, CancellationToken cancellationToken = default);

    [Post(Endpoints.StoreApi.Customers)]
    Task CreateCustomerAsync([Body] CreateCustomerApiRequest request, CancellationToken cancellationToken = default);
}

internal sealed record CreateCustomerApiRequest(string ExternalUserId);
