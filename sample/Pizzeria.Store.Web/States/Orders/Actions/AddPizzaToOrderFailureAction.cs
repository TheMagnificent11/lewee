namespace Pizzeria.Store.Web.States.Orders.Actions;

internal sealed record AddPizzaToOrderFailureAction(Guid PizzaId, string ErrorMessage);
