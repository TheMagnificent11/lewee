using Pizzeria.Store.Contracts;

namespace Pizzeria.Store.Web.States.Pizzas.Actions;

internal record LoadPizzasSuccessAction(PizzaDto[] Pizzas);
