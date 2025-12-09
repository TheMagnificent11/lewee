namespace Pizzeria.Store.Web.Orders.Actions;

public record AddPizzaToOrderFailureAction(Guid PizzaId, string ErrorMessage);
