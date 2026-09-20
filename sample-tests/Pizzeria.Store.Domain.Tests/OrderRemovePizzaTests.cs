using FluentAssertions;
using Lewee.Common;
using Xunit;

namespace Pizzeria.Store.Domain.Tests;

public sealed class OrderRemovePizzaTests
{
    [Fact]
    public void RemovePizza_WhenQuantityGreaterThanOne_DecrementsQuantity()
    {
        // Arrange
        var order = StartOrderWithPizza(out var pizza, quantity: 3);

        // Act
        var result = order.RemovePizza(pizza);

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Pizzas.Should().ContainSingle();
        order.Pizzas.Single().Quantity.Should().Be(2);
    }

    [Fact]
    public void RemovePizza_WhenQuantityIsOne_RemovesPizzaFromOrder()
    {
        // Arrange
        var order = StartOrderWithPizza(out var pizza, quantity: 1);

        // Act
        var result = order.RemovePizza(pizza);

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Pizzas.Should().BeEmpty();
    }

    [Fact]
    public void RemovePizza_WhenPizzaNotInOrder_ReturnsNotFound()
    {
        // Arrange
        var order = StartOrderWithPizza(out _, quantity: 1);
        var otherPizza = Menu.GetPizzaByName(Menu.PizzaNames.Carbonara);

        // Act
        var result = order.RemovePizza(otherPizza);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        order.Pizzas.Should().ContainSingle();
    }

    [Fact]
    public void RemovePizza_WhenOrderSubmitted_ReturnsBadRequest()
    {
        // Arrange
        var order = StartOrderWithPizza(out var pizza, quantity: 1);
        order.SubmitPickupOrder(Guid.NewGuid());

        // Act
        var result = order.RemovePizza(pizza);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        order.Pizzas.Should().ContainSingle();
    }

    [Fact]
    public void AddPizza_WhenOrderSubmitted_ReturnsBadRequest()
    {
        // Arrange
        var order = StartOrderWithPizza(out _, quantity: 1);
        var pizza = Menu.GetPizzaByName(Menu.PizzaNames.Carbonara);
        order.SubmitPickupOrder(Guid.NewGuid());

        // Act
        var result = order.AddPizza(pizza);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        order.Pizzas.Should().ContainSingle();
    }

    private static Order StartOrderWithPizza(out Pizza pizza, int quantity = 1)
    {
        var order = Order.StartNewOrder(Guid.NewGuid().ToString(), Guid.NewGuid());
        pizza = Menu.GetPizzaByName(Menu.PizzaNames.Margherita);

        for (var i = 0; i < quantity; i++)
        {
            order.AddPizza(pizza);
        }

        return order;
    }
}
