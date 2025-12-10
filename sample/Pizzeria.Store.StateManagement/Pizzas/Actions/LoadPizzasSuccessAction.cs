using Pizzeria.Store.Contracts.Pizzas;

namespace Pizzeria.Store.StateManagement.Pizzas.Actions;

public record LoadPizzasSuccessAction(IEnumerable<PizzaDto> Pizzas);
