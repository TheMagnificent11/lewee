using Pizzeria.Store.Contracts;

namespace Pizzeria.Store.WebClient.States.Pizzas.Actions;

public record LoadPizzasSuccessAction(PizzaDto[] Pizzas);
