using Pizzeria.Store.Contracts;

namespace Pizzeria.Store.Web.Pizzas.Actions;

public record LoadPizzasSuccessAction(IEnumerable<PizzaDto> Pizzas);
