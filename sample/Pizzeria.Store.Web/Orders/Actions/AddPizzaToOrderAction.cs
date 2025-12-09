namespace Pizzeria.Store.Web.Orders.Actions;

public record AddPizzaToOrderAction(Guid OrderId, Guid PizzaId);
