namespace Pizzeria.Store.Web.States.Orders.Actions;

internal record AddPizzaToOrderAction(Guid OrderId, Guid PizzaId);
