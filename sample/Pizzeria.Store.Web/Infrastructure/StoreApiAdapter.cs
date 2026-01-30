using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.Contracts.Pizzas;
using Pizzeria.Store.StateManagement;

namespace Pizzeria.Store.Infrastructure;

internal sealed class StoreApiAdapter : IStoreApi
{
    private readonly IStoreApiClient client;

    public StoreApiAdapter(IStoreApiClient client)
    {
        this.client = client;
    }

    public async Task<IEnumerable<PizzaDto>> GetPizzasAsync(CancellationToken cancellationToken = default)
    {
        return await this.client.GetPizzasAsync(cancellationToken);
    }

    public async Task<OrderDto?> StartOrderAsync(CancellationToken cancellationToken = default)
    {
        await this.client.StartOrderAsync(cancellationToken);

        return null;
    }

    public async Task AddPizzaToOrderAsync(Guid orderId, Guid pizzaId, CancellationToken cancellationToken = default)
    {
        await this.client.AddPizzaToOrderAsync(orderId, pizzaId, cancellationToken);
    }
}
