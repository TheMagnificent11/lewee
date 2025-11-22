namespace Pizzeria.Store.Web.States.Orders.Actions;

internal sealed record AddPizzaToOrderAction(Guid OrderId, Guid PizzaId);
