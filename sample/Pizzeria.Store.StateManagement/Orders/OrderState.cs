using Lewee.Infrastructure.Fluxor;
using Pizzeria.Store.Contracts.Orders;

namespace Pizzeria.Store.StateManagement.Orders;

public record OrderState : RequestState<OrderDto>;
