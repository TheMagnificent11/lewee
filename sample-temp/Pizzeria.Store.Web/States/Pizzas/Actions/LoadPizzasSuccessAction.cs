using Pizzeria.Store.Contracts;

namespace Pizzeria.Store.Web.States.Pizzas.Actions;

public record LoadPizzasSuccessAction(IEnumerable<PizzaDto> Pizzas);
