using Lewee.Infrastructure.Fluxor;
using Pizzeria.Store.Contracts.Orders;

namespace Pizzeria.Ordering.StateManagement.Orders;

public record OrderState : RequestState<OrderDto>;
