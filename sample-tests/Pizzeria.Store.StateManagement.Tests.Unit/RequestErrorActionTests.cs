using FluentAssertions;
using Lewee.StateManagement;
using Pizzeria.Store.StateManagement.Orders.Actions;
using Pizzeria.Store.StateManagement.Pizzas.Actions;
using Xunit;

namespace Pizzeria.Store.StateManagement.Tests.Unit;

public class RequestErrorActionTests
{
    [Fact]
    public void StartOrderFailureAction_ShouldImplementIRequestErrorAction()
    {
        // Arrange & Act
        var action = new StartOrderFailureAction();

        // Assert
        action.Should().BeAssignableTo<IRequestErrorAction>();
        action.Should().BeAssignableTo<IRequestAction>();
    }

    [Fact]
    public void StartOrderFailureAction_ShouldSetCorrelationIdAndErrorMessage()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var errorMessage = "Failed to start order";

        // Act
        var action = new StartOrderFailureAction
        {
            CorrelationId = correlationId,
            ErrorMessage = errorMessage,
        };

        // Assert
        action.CorrelationId.Should().Be(correlationId);
        action.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public void AddPizzaToOrderFailureAction_ShouldImplementIRequestErrorAction()
    {
        // Arrange & Act
        var action = new AddPizzaToOrderFailureAction();

        // Assert
        action.Should().BeAssignableTo<IRequestErrorAction>();
        action.Should().BeAssignableTo<IRequestAction>();
    }

    [Fact]
    public void AddPizzaToOrderFailureAction_ShouldSetCorrelationIdAndErrorMessage()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var errorMessage = "Failed to add pizza to order";

        // Act
        var action = new AddPizzaToOrderFailureAction
        {
            CorrelationId = correlationId,
            ErrorMessage = errorMessage,
        };

        // Assert
        action.CorrelationId.Should().Be(correlationId);
        action.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public void LoadPizzasFailureAction_ShouldImplementIRequestErrorAction()
    {
        // Arrange & Act
        var action = new LoadPizzasFailureAction();

        // Assert
        action.Should().BeAssignableTo<IRequestErrorAction>();
        action.Should().BeAssignableTo<IRequestAction>();
    }

    [Fact]
    public void LoadPizzasFailureAction_ShouldSetCorrelationIdAndErrorMessage()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var errorMessage = "Failed to load pizzas";

        // Act
        var action = new LoadPizzasFailureAction
        {
            CorrelationId = correlationId,
            ErrorMessage = errorMessage,
        };

        // Assert
        action.CorrelationId.Should().Be(correlationId);
        action.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public void AddPizzaToOrderFailureAction_ShouldHaveDefaultEmptyErrorMessage()
    {
        // Arrange & Act
        var action = new AddPizzaToOrderFailureAction();

        // Assert
        action.ErrorMessage.Should().Be(string.Empty);
    }
}
