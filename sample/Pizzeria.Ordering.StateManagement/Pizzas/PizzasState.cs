using Lewee.Infrastructure.Fluxor;
using Pizzeria.Store.Contracts.Pizzas;

namespace Pizzeria.Ordering.StateManagement.Pizzas;

public record PizzasState : RequestState<IEnumerable<PizzaDto>>;
