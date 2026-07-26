using FluentAssertions;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.StateManagement.Orders;
using Pizzeria.Store.StateManagement.Orders.Actions;
using Xunit;

namespace Pizzeria.Store.StateManagement.Tests.Unit;

public class OrdersReducerTests
{
    [Fact]
    public void OnStartOrder_SetsIsSavingToTrue()
    {
        // Arrange
        var state = new OrderState();
        var correlationId = Guid.NewGuid();
        var action = new StartOrderAction { CorrelationId = correlationId };

        // Act
        var result = OrderReducer.OnStartOrder(state, action);

        // Assert
        result.IsSaving.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
        result.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public void OnStartOrder_ClearsExistingErrorMessage()
    {
        // Arrange
        var state = new OrderState { ErrorMessage = "Previous error" };
        var action = new StartOrderAction { CorrelationId = Guid.NewGuid() };

        // Act
        var result = OrderReducer.OnStartOrder(state, action);

        // Assert
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void OnStartOrderSuccess_SetsIsSavingToFalse()
    {
        // Arrange
        var state = new OrderState { IsSaving = true };
        var correlationId = Guid.NewGuid();
        var action = new StartOrderSuccessAction { CorrelationId = correlationId };

        // Act
        var result = OrderReducer.OnStartOrderSuccess(state, action);

        // Assert
        result.IsSaving.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public void OnStartOrderFailure_SetsErrorMessage()
    {
        // Arrange
        var state = new OrderState { IsSaving = true };
        var correlationId = Guid.NewGuid();
        var errorMessage = "Failed to start order";
        var action = new StartOrderFailureAction
        {
            CorrelationId = correlationId,
            ErrorMessage = errorMessage,
        };

        // Act
        var result = OrderReducer.OnStartOrderFailure(state, action);

        // Assert
        result.IsSaving.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
        result.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public void OnStartOrderCompleted_SetsCurrentOrder()
    {
        // Arrange
        var state = new OrderState();
        var correlationId = Guid.NewGuid();
        var order = new OrderDto
        {
            Id = Guid.NewGuid(),
            UserId = "test-user",
            StartedDateTime = DateTime.UtcNow,
            Pizzas = [],
            TotalCost = 0,
        };
        var action = new StartOrderCompletedAction
        {
            Data = order,
            CorrelationId = correlationId,
        };

        // Act
        var result = OrderReducer.OnStartOrderCompleted(state, action);

        // Assert
        result.Data.Should().Be(order);
        result.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public void OnAddPizzaToOrderSuccess_ClearsErrorMessage()
    {
        // Arrange
        var state = new OrderState { ErrorMessage = "Previous error" };
        var action = new AddPizzaToOrderSuccessAction { CorrelationId = Guid.NewGuid() };

        // Act
        var result = OrderReducer.OnAddPizzaToOrderSuccess(state, action);

        // Assert
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void OnAddPizzaToOrderFailure_SetsErrorMessage()
    {
        // Arrange
        var state = new OrderState();
        var errorMessage = "Failed to add pizza";
        var action = new AddPizzaToOrderFailureAction
        {
            CorrelationId = Guid.NewGuid(),
            ErrorMessage = errorMessage,
        };

        // Act
        var result = OrderReducer.OnAddPizzaToOrderFailure(state, action);

        // Assert
        result.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public void OnClearOrderError_ClearsErrorMessage()
    {
        // Arrange
        var state = new OrderState { ErrorMessage = "Some error" };
        var action = new ClearOrderErrorAction();

        // Act
        var result = OrderReducer.OnClearOrderError(state, action);

        // Assert
        result.ErrorMessage.Should().BeNull();
    }
}
