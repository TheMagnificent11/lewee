namespace Pizzeria.Store.WebClient.States.Orders.Actions;

public record AddPizzaToOrderFailureAction(Guid PizzaId, string ErrorMessage);
