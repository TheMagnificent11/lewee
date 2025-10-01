using Pizzeria.Store.Contracts;

namespace Pizzeria.Store.WebClient.States.Pizzas.Actions;

public record LoadPizzasAction;

public record LoadPizzasSuccessAction(PizzaDto[] Pizzas);

public record LoadPizzasFailureAction(string ErrorMessage);