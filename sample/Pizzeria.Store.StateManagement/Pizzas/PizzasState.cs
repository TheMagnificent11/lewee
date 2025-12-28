using Lewee.Infrastructure.Fluxor;
using Pizzeria.Store.Contracts.Pizzas;

namespace Pizzeria.Store.StateManagement.Pizzas;

public record PizzasState : RequestState<IEnumerable<PizzaDto>>;
