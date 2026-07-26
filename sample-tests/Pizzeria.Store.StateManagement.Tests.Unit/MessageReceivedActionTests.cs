using FluentAssertions;
using Lewee.Infrastructure.Fluxor;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.StateManagement.Orders.Actions;
using Xunit;

namespace Pizzeria.Store.StateManagement.Tests.Unit;

public class MessageReceivedActionTests
{
    [Fact]
    public void StartOrderCompletedAction_ShouldImplementIMessageReceivedAction()
    {
        // Arrange & Act
        var action = new StartOrderCompletedAction();

        // Assert
        action.Should().BeAssignableTo<IMessageReceivedAction<OrderDto>>();
        action.Should().BeAssignableTo<IMessageReceivedAction>();
        action.Should().BeAssignableTo<IRequestAction>();
    }

    [Fact]
    public void StartOrderCompletedAction_ShouldSetCorrelationId()
    {
        // Arrange
        var correlationId = Guid.NewGuid();

        // Act
        var action = new StartOrderCompletedAction { CorrelationId = correlationId };

        // Assert
        action.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public void StartOrderCompletedAction_ShouldSetData()
    {
        // Arrange
        var order = new OrderDto
        {
            Id = Guid.NewGuid(),
            UserId = "test-user",
            StartedDateTime = DateTime.UtcNow,
            Pizzas = [],
            TotalCost = 25.99m,
        };

        // Act
        var action = new StartOrderCompletedAction { Data = order };

        // Assert
        action.Data.Should().Be(order);
    }

    [Fact]
    public void StartOrderCompletedAction_ShouldSetAllProperties()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var order = new OrderDto
        {
            Id = Guid.NewGuid(),
            UserId = "test-user-2",
            StartedDateTime = DateTime.UtcNow,
            SubmittedDateTime = DateTime.UtcNow.AddMinutes(5),
            Pizzas =
            [
                new OrderPizzaDto
                {
                    Id = Guid.NewGuid(),
                    PizzaId = Guid.NewGuid(),
                    PizzaName = "Margherita",
                    PizzaPrice = 12.99m,
                    Quantity = 2,
                    LineTotal = 25.98m,
                },
            ],
            TotalCost = 25.98m,
        };

        // Act
        var action = new StartOrderCompletedAction
        {
            CorrelationId = correlationId,
            Data = order,
        };

        // Assert
        action.CorrelationId.Should().Be(correlationId);
        action.Data.Should().Be(order);
        action.Data.Pizzas.Should().ContainSingle();
    }

    [Fact]
    public void AddPizzaToOrderCompletedAction_ShouldImplementIMessageReceivedAction()
    {
        // Arrange & Act
        var action = new AddPizzaToOrderCompletedAction();

        // Assert
        action.Should().BeAssignableTo<IMessageReceivedAction<OrderDto>>();
        action.Should().BeAssignableTo<IMessageReceivedAction>();
        action.Should().BeAssignableTo<IRequestAction>();
    }

    [Fact]
    public void AddPizzaToOrderCompletedAction_ShouldSetCorrelationIdAndData()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var order = new OrderDto
        {
            Id = Guid.NewGuid(),
            UserId = "test-user",
            StartedDateTime = DateTime.UtcNow,
            Pizzas = [],
            TotalCost = 0m,
        };

        // Act
        var action = new AddPizzaToOrderCompletedAction
        {
            CorrelationId = correlationId,
            Data = order,
        };

        // Assert
        action.CorrelationId.Should().Be(correlationId);
        action.Data.Should().Be(order);
    }
}
