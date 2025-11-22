using Pizzeria.Store.Contracts;

namespace Pizzeria.Store.Web.States.Pizzas.Actions;

internal sealed record LoadPizzasSuccessAction(PizzaDto[] Pizzas);
