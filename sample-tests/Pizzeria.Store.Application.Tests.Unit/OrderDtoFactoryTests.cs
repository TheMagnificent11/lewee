using FluentAssertions;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Domain;
using Xunit;

namespace Pizzeria.Store.Application.Tests.Unit;

public sealed class OrderDtoFactoryTests
{
    [Fact]
    public void Should_UseSnapshotPizzaDetails_When_CreatingOrderDto()
    {
        var pizza = Pizza.Create("Margherita", "Classic", 12.50m);
        var order = Order.StartNewOrder(TestHelpers.OwnerUserId, Guid.NewGuid());
        order.AddPizza(pizza, Guid.NewGuid());
        pizza.UpdateDetails("Updated Margherita", "Updated description", 15m);

        var orderDto = OrderDtoFactory.Create(order);

        orderDto.Pizzas.Should().ContainSingle()
            .Which.Should().Match<Pizzeria.Store.Contracts.Orders.OrderPizzaDto>(item =>
                item.PizzaName == "Margherita" &&
                item.PizzaPrice == 12.50m &&
                item.LineTotal == 12.50m);
        orderDto.TotalCost.Should().Be(12.50m);
    }
}
