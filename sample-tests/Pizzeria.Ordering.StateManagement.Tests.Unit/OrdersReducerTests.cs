using System.Collections.Immutable;
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
        var correlationId = Guid.NewGuid();
        var state = new OrderState { IsSaving = true, CorrelationId = correlationId };
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
        var correlationId = Guid.NewGuid();
        var state = new OrderState { IsSaving = true, CorrelationId = correlationId };
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
        var correlationId = Guid.NewGuid();
        var state = new OrderState { CorrelationId = correlationId };
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
        var correlationId = Guid.NewGuid();
        var state = new OrderState { ErrorMessage = "Previous error", CorrelationId = correlationId };
        var action = new AddPizzaToOrderSuccessAction { CorrelationId = correlationId };

        // Act
        var result = OrderReducer.OnAddPizzaToOrderSuccess(state, action);

        // Assert
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void OnAddPizzaToOrderFailure_SetsErrorMessage()
    {
        // Arrange
        var errorMessage = "Failed to add pizza";
        var correlationId = Guid.NewGuid();
        var state = new OrderState { CorrelationId = correlationId };
        var action = new AddPizzaToOrderFailureAction
        {
            CorrelationId = correlationId,
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
    public void OnOrderCommandSuccess_DoesNotOverwriteNewerCommandCorrelation()
    {
        var firstCorrelationId = Guid.NewGuid();
        var secondCorrelationId = Guid.NewGuid();
        var state = OrderReducer.OnAddPizzaToOrder(new OrderState(), new AddPizzaToOrderAction
        {
            CorrelationId = firstCorrelationId,
        });
        state = OrderReducer.OnRemovePizzaFromOrder(state, new RemovePizzaFromOrderAction
        {
            CorrelationId = secondCorrelationId,
        });

        var result = OrderReducer.OnAddPizzaToOrderSuccess(state, new AddPizzaToOrderSuccessAction
        {
            CorrelationId = firstCorrelationId,
        });

        result.CorrelationId.Should().Be(secondCorrelationId);
        result.PendingCommands.Should().ContainKey(firstCorrelationId);
        result.PendingCommands.Should().ContainKey(secondCorrelationId);
        result.IsSaving.Should().BeTrue();
    }

    [Fact]
    public void OnOrderUpdated_ReplacesCurrentOrderWithoutChangingPendingCommands()
    {
        var orderId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var pendingCorrelationId = Guid.NewGuid();
        var state = new OrderState
        {
            Data = new OrderDto { Id = orderId },
            CorrelationId = pendingCorrelationId,
            PendingCommands = ImmutableDictionary<Guid, OrderCommandType>.Empty
                .Add(pendingCorrelationId, OrderCommandType.SubmitPickupOrder),
        };
        var updatedOrder = new OrderDto { Id = orderId, Status = OrderStatus.Making };

        var result = OrderReducer.OnOrderUpdated(
            state,
            new OrderUpdatedAction { Data = updatedOrder, CorrelationId = correlationId });

        result.Data.Should().Be(updatedOrder);
        result.CorrelationId.Should().Be(pendingCorrelationId);
        result.PendingCommands.Should().ContainKey(pendingCorrelationId);
    }

    [Fact]
    public void OnRemovePizzaFromOrderSuccess_SetsIsSavingToFalse()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var state = new OrderState { IsSaving = true, CorrelationId = correlationId };
        var action = new RemovePizzaFromOrderSuccessAction { CorrelationId = correlationId };

        // Act
        var result = OrderReducer.OnRemovePizzaFromOrderSuccess(state, action);

        // Assert
        result.IsSaving.Should().BeFalse();
    }

    [Fact]
    public void OnRemovePizzaFromOrderFailure_SetsErrorMessage()
    {
        // Arrange
        var errorMessage = "Failed to remove pizza";
        var correlationId = Guid.NewGuid();
        var state = new OrderState { CorrelationId = correlationId };
        var action = new RemovePizzaFromOrderFailureAction
        {
            CorrelationId = correlationId,
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
        var correlationId = Guid.NewGuid();
        var state = new OrderState { IsSaving = true, CorrelationId = correlationId };
        var action = new SubmitPickupOrderSuccessAction { CorrelationId = correlationId };

        // Act
        var result = OrderReducer.OnSubmitPickupOrderSuccess(state, action);

        // Assert
        result.IsSaving.Should().BeFalse();
    }

    [Fact]
    public void OnSubmitPickupOrderFailure_SetsErrorMessage()
    {
        // Arrange
        var errorMessage = "Failed to submit pickup order";
        var correlationId = Guid.NewGuid();
        var state = new OrderState { CorrelationId = correlationId };
        var action = new SubmitPickupOrderFailureAction
        {
            CorrelationId = correlationId,
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
        var correlationId = Guid.NewGuid();
        var state = new OrderState { IsSaving = true, CorrelationId = correlationId };
        var action = new SubmitDeliveryOrderSuccessAction { CorrelationId = correlationId };

        // Act
        var result = OrderReducer.OnSubmitDeliveryOrderSuccess(state, action);

        // Assert
        result.IsSaving.Should().BeFalse();
    }

    [Fact]
    public void OnSubmitDeliveryOrderFailure_SetsErrorMessage()
    {
        // Arrange
        var errorMessage = "Failed to submit delivery order";
        var correlationId = Guid.NewGuid();
        var state = new OrderState { CorrelationId = correlationId };
        var action = new SubmitDeliveryOrderFailureAction
        {
            CorrelationId = correlationId,
            ErrorMessage = errorMessage,
        };

        // Act
        var result = OrderReducer.OnSubmitDeliveryOrderFailure(state, action);

        // Assert
        result.ErrorMessage.Should().Be(errorMessage);
    }
}
