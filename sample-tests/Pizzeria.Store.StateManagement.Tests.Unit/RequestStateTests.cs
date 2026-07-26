using FluentAssertions;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.Contracts.Pizzas;
using Pizzeria.Store.StateManagement.Orders;
using Pizzeria.Store.StateManagement.Pizzas;
using Xunit;

namespace Pizzeria.Store.StateManagement.Tests.Unit;

public class RequestStateTests
{
    [Fact]
    public void OrderState_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var state = new OrderState();

        // Assert
        state.IsLoading.Should().BeFalse();
        state.IsSaving.Should().BeFalse();
        state.CorrelationId.Should().Be(Guid.Empty);
        state.Data.Should().BeNull();
        state.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void OrderState_WithData_ShouldSetProperties()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var order = new OrderDto
        {
            Id = Guid.NewGuid(),
            UserId = "test-user",
            StartedDateTime = DateTime.UtcNow,
            Pizzas = [],
            TotalCost = 25.99m,
        };

        // Act
        var state = new OrderState
        {
            IsLoading = true,
            IsSaving = true,
            CorrelationId = correlationId,
            Data = order,
            ErrorMessage = "Test error",
        };

        // Assert
        state.IsLoading.Should().BeTrue();
        state.IsSaving.Should().BeTrue();
        state.CorrelationId.Should().Be(correlationId);
        state.Data.Should().Be(order);
        state.ErrorMessage.Should().Be("Test error");
    }

    [Fact]
    public void PizzasState_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var state = new PizzasState();

        // Assert
        state.IsLoading.Should().BeFalse();
        state.IsSaving.Should().BeFalse();
        state.CorrelationId.Should().Be(Guid.Empty);
        state.Data.Should().BeNull();
        state.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void PizzasState_WithData_ShouldSetProperties()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var pizzas = new List<PizzaDto>
        {
            new(Guid.NewGuid(), "Margherita", "Classic tomato and mozzarella", 12.99m),
            new(Guid.NewGuid(), "Pepperoni", "Topped with spicy pepperoni", 14.99m),
        };

        // Act
        var state = new PizzasState
        {
            IsLoading = true,
            CorrelationId = correlationId,
            Data = pizzas,
        };

        // Assert
        state.IsLoading.Should().BeTrue();
        state.CorrelationId.Should().Be(correlationId);
        state.Data.Should().BeEquivalentTo(pizzas);
    }

    [Fact]
    public void RequestState_RecordEquality_ShouldWork()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var state1 = new OrderState
        {
            IsLoading = true,
            CorrelationId = correlationId,
        };
        var state2 = new OrderState
        {
            IsLoading = true,
            CorrelationId = correlationId,
        };

        // Assert
        state1.Should().Be(state2);
    }

    [Fact]
    public void RequestState_RecordWith_ShouldCreateNewInstance()
    {
        // Arrange
        var state = new OrderState
        {
            IsLoading = true,
            CorrelationId = Guid.NewGuid(),
        };

        // Act
        var newState = state with { IsLoading = false };

        // Assert
        newState.IsLoading.Should().BeFalse();
        state.IsLoading.Should().BeTrue();
        newState.CorrelationId.Should().Be(state.CorrelationId);
    }
}
