using FluentAssertions;
using Lewee.StateManagement;
using Pizzeria.Store.StateManagement.Orders.Actions;
using Pizzeria.Store.StateManagement.Pizzas.Actions;
using Xunit;

namespace Pizzeria.Store.StateManagement.Tests.Unit;

public class RequestActionTests
{
    [Fact]
    public void StartOrderAction_ShouldImplementIRequestAction()
    {
        // Arrange & Act
        var action = new StartOrderAction();

        // Assert
        action.Should().BeAssignableTo<IRequestAction>();
    }

    [Fact]
    public void StartOrderAction_ShouldGenerateDefaultCorrelationId()
    {
        // Arrange & Act
        var action = new StartOrderAction();

        // Assert
        action.CorrelationId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void StartOrderAction_ShouldAcceptCustomCorrelationId()
    {
        // Arrange
        var correlationId = Guid.NewGuid();

        // Act
        var action = new StartOrderAction { CorrelationId = correlationId };

        // Assert
        action.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public void LoadPizzasAction_ShouldImplementIRequestAction()
    {
        // Arrange & Act
        var action = new LoadPizzasAction();

        // Assert
        action.Should().BeAssignableTo<IRequestAction>();
    }

    [Fact]
    public void LoadPizzasAction_ShouldGenerateDefaultCorrelationId()
    {
        // Arrange & Act
        var action = new LoadPizzasAction();

        // Assert
        action.CorrelationId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void LoadPizzasAction_ShouldAcceptCustomCorrelationId()
    {
        // Arrange
        var correlationId = Guid.NewGuid();

        // Act
        var action = new LoadPizzasAction { CorrelationId = correlationId };

        // Assert
        action.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public void AddPizzaToOrderAction_ShouldImplementIRequestAction()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var pizzaId = Guid.NewGuid();

        // Act
        var action = new AddPizzaToOrderAction
        {
            CorrelationId = correlationId,
            OrderId = orderId,
            PizzaId = pizzaId,
        };

        // Assert
        action.Should().BeAssignableTo<IRequestAction>();
        action.CorrelationId.Should().Be(correlationId);
        action.OrderId.Should().Be(orderId);
        action.PizzaId.Should().Be(pizzaId);
    }

    [Fact]
    public void AddPizzaToOrderAction_ShouldGenerateDefaultCorrelationId()
    {
        // Arrange & Act
        var action = new AddPizzaToOrderAction();

        // Assert
        action.CorrelationId.Should().NotBe(Guid.Empty);
    }
}
