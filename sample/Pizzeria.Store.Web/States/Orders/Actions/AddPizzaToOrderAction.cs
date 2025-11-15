namespace Pizzeria.Store.Web.States.Orders.Actions;

public record AddPizzaToOrderAction(Guid OrderId, Guid PizzaId);
