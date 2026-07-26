using FluentAssertions;
using Lewee.Infrastructure.Fluxor;
using Pizzeria.Store.StateManagement.Orders.Actions;
using Xunit;

namespace Pizzeria.Store.StateManagement.Tests.Unit;

public class RequestSuccessActionTests
{
    [Fact]
    public void StartOrderSuccessAction_ShouldImplementIRequestSuccessAction()
    {
        // Arrange & Act
        var action = new StartOrderSuccessAction();

        // Assert
        action.Should().BeAssignableTo<IRequestSuccessAction>();
        action.Should().BeAssignableTo<IRequestAction>();
    }

    [Fact]
    public void StartOrderSuccessAction_ShouldSetCorrelationId()
    {
        // Arrange
        var correlationId = Guid.NewGuid();

        // Act
        var action = new StartOrderSuccessAction { CorrelationId = correlationId };

        // Assert
        action.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public void AddPizzaToOrderSuccessAction_ShouldImplementIRequestSuccessAction()
    {
        // Arrange & Act
        var action = new AddPizzaToOrderSuccessAction();

        // Assert
        action.Should().BeAssignableTo<IRequestSuccessAction>();
        action.Should().BeAssignableTo<IRequestAction>();
    }

    [Fact]
    public void AddPizzaToOrderSuccessAction_ShouldSetCorrelationId()
    {
        // Arrange
        var correlationId = Guid.NewGuid();

        // Act
        var action = new AddPizzaToOrderSuccessAction { CorrelationId = correlationId };

        // Assert
        action.CorrelationId.Should().Be(correlationId);
    }
}
