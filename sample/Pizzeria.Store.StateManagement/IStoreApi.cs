using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.Contracts.Pizzas;

namespace Pizzeria.Store.StateManagement;

public interface IStoreApi
{
    Task<IEnumerable<PizzaDto>> GetPizzasAsync(CancellationToken cancellationToken = default);

    Task<OrderDto?> StartOrderAsync(CancellationToken cancellationToken = default);

    Task AddPizzaToOrderAsync(Guid orderId, Guid pizzaId, CancellationToken cancellationToken = default);
}
