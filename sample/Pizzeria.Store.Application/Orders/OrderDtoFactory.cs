using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.Domain;

namespace Pizzeria.Store.Application.Orders;

internal static class OrderDtoFactory
{
    public static OrderDto Create(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        var orderLines = order.Pizzas
            .Select(op => new OrderPizzaDto
            {
                Id = op.Id,
                PizzaId = op.PizzaId,
                PizzaName = op.Pizza.Name,
                PizzaPrice = op.Pizza.Price,
                Quantity = op.Quantity,
                LineTotal = op.Pizza.Price * op.Quantity,
            })
            .ToArray();

        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            Status = order.Status,
            StartedDateTime = order.StartedDateTime,
            SubmittedDateTime = order.SubmittedDateTime,
            PreparedDateTime = order.PreparedDateTime,
            CompletedDateTime = order.CompletedDateTime,
            DeliveryAddress = order.DeliveryAddress,
            IsDeliveryOrder = order.IsDeliveryOrder,
            Pizzas = orderLines,
            TotalCost = orderLines.Sum(p => p.LineTotal),
        };
    }
}
