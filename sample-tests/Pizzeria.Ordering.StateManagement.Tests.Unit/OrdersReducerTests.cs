using FluentAssertions;
using Pizzeria.Ordering.StateManagement.Orders;
using Pizzeria.Ordering.StateManagement.Orders.Actions;
using Pizzeria.Store.Contracts.Orders;
using Xunit;

namespace Pizzeria.Ordering.StateManagement.Tests.Unit;

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

    [Fact]
    public void OnRemovePizzaFromOrder_SetsIsSavingToTrueAndKeepsData()
    {
        // Arrange
        var order = new OrderDto { Id = Guid.NewGuid() };
        var state = new OrderState { Data = order };
        var action = new RemovePizzaFromOrderAction { CorrelationId = Guid.NewGuid() };

        // Act
        var result = OrderReducer.OnRemovePizzaFromOrder(state, action);

        // Assert
        result.IsSaving.Should().BeTrue();
        result.Data.Should().Be(order);
    }

    [Fact]
    public void OnRemovePizzaFromOrderSuccess_SetsIsSavingToFalse()
    {
        // Arrange
        var state = new OrderState { IsSaving = true };
        var action = new RemovePizzaFromOrderSuccessAction { CorrelationId = Guid.NewGuid() };

        // Act
        var result = OrderReducer.OnRemovePizzaFromOrderSuccess(state, action);

        // Assert
        result.IsSaving.Should().BeFalse();
    }

    [Fact]
    public void OnRemovePizzaFromOrderFailure_SetsErrorMessage()
    {
        // Arrange
        var state = new OrderState();
        var errorMessage = "Failed to remove pizza";
        var action = new RemovePizzaFromOrderFailureAction
        {
            CorrelationId = Guid.NewGuid(),
            ErrorMessage = errorMessage,
        };

        // Act
        var result = OrderReducer.OnRemovePizzaFromOrderFailure(state, action);

        // Assert
        result.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public void OnSubmitPickupOrder_SetsIsSavingToTrue()
    {
        // Arrange
        var state = new OrderState();
        var action = new SubmitPickupOrderAction { CorrelationId = Guid.NewGuid() };

        // Act
        var result = OrderReducer.OnSubmitPickupOrder(state, action);

        // Assert
        result.IsSaving.Should().BeTrue();
    }

    [Fact]
    public void OnSubmitPickupOrderSuccess_SetsIsSavingToFalse()
    {
        // Arrange
        var state = new OrderState { IsSaving = true };
        var action = new SubmitPickupOrderSuccessAction { CorrelationId = Guid.NewGuid() };

        // Act
        var result = OrderReducer.OnSubmitPickupOrderSuccess(state, action);

        // Assert
        result.IsSaving.Should().BeFalse();
    }

    [Fact]
    public void OnSubmitPickupOrderFailure_SetsErrorMessage()
    {
        // Arrange
        var state = new OrderState();
        var errorMessage = "Failed to submit pickup order";
        var action = new SubmitPickupOrderFailureAction
        {
            CorrelationId = Guid.NewGuid(),
            ErrorMessage = errorMessage,
        };

        // Act
        var result = OrderReducer.OnSubmitPickupOrderFailure(state, action);

        // Assert
        result.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public void OnSubmitDeliveryOrder_SetsIsSavingToTrue()
    {
        // Arrange
        var state = new OrderState();
        var action = new SubmitDeliveryOrderAction { CorrelationId = Guid.NewGuid() };

        // Act
        var result = OrderReducer.OnSubmitDeliveryOrder(state, action);

        // Assert
        result.IsSaving.Should().BeTrue();
    }

    [Fact]
    public void OnSubmitDeliveryOrderSuccess_SetsIsSavingToFalse()
    {
        // Arrange
        var state = new OrderState { IsSaving = true };
        var action = new SubmitDeliveryOrderSuccessAction { CorrelationId = Guid.NewGuid() };

        // Act
        var result = OrderReducer.OnSubmitDeliveryOrderSuccess(state, action);

        // Assert
        result.IsSaving.Should().BeFalse();
    }

    [Fact]
    public void OnSubmitDeliveryOrderFailure_SetsErrorMessage()
    {
        // Arrange
        var state = new OrderState();
        var errorMessage = "Failed to submit delivery order";
        var action = new SubmitDeliveryOrderFailureAction
        {
            CorrelationId = Guid.NewGuid(),
            ErrorMessage = errorMessage,
        };

        // Act
        var result = OrderReducer.OnSubmitDeliveryOrderFailure(state, action);

        // Assert
        result.ErrorMessage.Should().Be(errorMessage);
    }
}
