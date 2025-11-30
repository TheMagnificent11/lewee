namespace Pizzeria.Store.Web.States.Orders.Actions;

public record AddPizzaToOrderFailureAction(Guid PizzaId, string ErrorMessage);
