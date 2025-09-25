#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using FluentAssertions;
using Pizzeria.Store.Domain;
using Xunit;

namespace Aspire.Pizzeria.Domain.Tests;

public sealed class OrderTests
{
    [Fact]
    public void SubmitPickupOrder()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var margherita = Menu.GetPizzaByName(Menu.PizzaNames.Margherita);
        var fourCheeses = Menu.GetPizzaByName(Menu.PizzaNames.QuattroFormaggi);
        var now = DateTime.UtcNow;

        // Act
        var order = Order.StartNewOrder(userId);

        order.AddPizza(margherita);
        order.AddPizza(fourCheeses);
        order.AddPizza(fourCheeses);

        order.SubmitPickupOrder();

        // Assert
        order.Id.Should().NotBeEmpty();
        order.UserId.Should().Be(userId);
        order.SubmittedDateTime.Should().BeCloseTo(now, precision: TimeSpan.FromMilliseconds(500));
        order.DeliveryAddress.Should().BeNull();
        order.Pizzas.Should().HaveCount(2);

        foreach (var item in order.Pizzas)
        {
            item.Id.Should().NotBeEmpty();
            item.OrderId.Should().Be(order.Id);

            if (item.PizzaId == margherita.Id)
            {
                item.Quantity.Should().Be(1);
            }
            else if (item.PizzaId == fourCheeses.Id)
            {
                item.Quantity.Should().Be(2);
            }
            else
            {
                Assert.Fail("Unexpected pizza in the order.");
            }
        }

        order.IsDeliveryOrder.Should().BeFalse();
        order.IsPrepared.Should().BeFalse();
        order.PreparedDateTime.Should().BeNull();
        order.IsCompleted.Should().BeFalse();
        order.CompletedDateTime.Should().BeNull();
    }
}
