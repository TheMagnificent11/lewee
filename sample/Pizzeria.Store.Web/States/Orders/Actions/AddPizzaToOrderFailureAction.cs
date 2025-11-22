namespace Pizzeria.Store.Web.States.Orders.Actions;

internal record AddPizzaToOrderFailureAction(Guid PizzaId, string ErrorMessage);
