namespace Pizzeria.Store.WebClient.States.Orders.Actions;

public record AddPizzaToOrderAction(Guid OrderId, Guid PizzaId);
