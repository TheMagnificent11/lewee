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
        var result = order.RemovePizza(pizza, Guid.NewGuid());

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
        var result = order.RemovePizza(pizza, Guid.NewGuid());

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
        var result = order.RemovePizza(otherPizza, Guid.NewGuid());

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
        var result = order.RemovePizza(pizza, Guid.NewGuid());

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
        var result = order.AddPizza(pizza, Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        order.Pizzas.Should().ContainSingle();
    }

    [Fact]
    public void AddPizza_WhenSuccessful_RaisesPizzaAddedToOrderEvent()
    {
        // Arrange
        var order = StartOrderWithPizza(out var pizza, quantity: 1);
        order.DomainEvents.GetAndClear();
        var correlationId = Guid.NewGuid();

        // Act
        order.AddPizza(pizza, correlationId);

        // Assert
        var domainEvent = order.DomainEvents.GetAndClear().Should().ContainSingle().Subject;
        domainEvent.Should().BeOfType<PizzaAddedToOrderEvent>();
        var pizzaAddedEvent = (PizzaAddedToOrderEvent)domainEvent;
        pizzaAddedEvent.OrderId.Should().Be(order.Id);
        pizzaAddedEvent.UserId.Should().Be(order.UserId);
        pizzaAddedEvent.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public void RemovePizza_WhenSuccessful_RaisesPizzaRemovedFromOrderEvent()
    {
        // Arrange
        var order = StartOrderWithPizza(out var pizza, quantity: 1);
        order.DomainEvents.GetAndClear();
        var correlationId = Guid.NewGuid();

        // Act
        order.RemovePizza(pizza, correlationId);

        // Assert
        var domainEvent = order.DomainEvents.GetAndClear().Should().ContainSingle().Subject;
        domainEvent.Should().BeOfType<PizzaRemovedFromOrderEvent>();
        var pizzaRemovedEvent = (PizzaRemovedFromOrderEvent)domainEvent;
        pizzaRemovedEvent.OrderId.Should().Be(order.Id);
        pizzaRemovedEvent.UserId.Should().Be(order.UserId);
        pizzaRemovedEvent.CorrelationId.Should().Be(correlationId);
    }

    private static Order StartOrderWithPizza(out Pizza pizza, int quantity = 1)
    {
        var order = Order.StartNewOrder(Guid.NewGuid().ToString(), Guid.NewGuid());
        pizza = Menu.GetPizzaByName(Menu.PizzaNames.Margherita);

        for (var i = 0; i < quantity; i++)
        {
            order.AddPizza(pizza, Guid.NewGuid());
        }

        return order;
    }
}
