using Pizzeria.Store.Contracts.Pizzas;

namespace Pizzeria.Store.Contracts.Pizzas.Actions;

public record LoadPizzasSuccessAction(IEnumerable<PizzaDto> Pizzas);
