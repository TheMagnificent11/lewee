namespace Pizzeria.Store.StateManagement.Orders.Actions;

public record AddPizzaToOrderFailureAction(Guid PizzaId, string ErrorMessage);
