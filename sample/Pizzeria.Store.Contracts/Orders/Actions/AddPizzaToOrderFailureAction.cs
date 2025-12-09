namespace Pizzeria.Store.Contracts.Orders.Actions;

public record AddPizzaToOrderFailureAction(Guid PizzaId, string ErrorMessage);
