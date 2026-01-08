using FluentAssertions;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.StateManagement.Orders;
using Xunit;

namespace Lewee.Infrastructure.Fluxor.Tests.Unit;

public class RequestStateTests
{
    [Fact]
    public void RequestState_DefaultValues_ShouldBeCorrect()
    {
        var state = new OrderState();

        state.IsLoading.Should().BeFalse();
        state.IsSaving.Should().BeFalse();
        state.CorrelationId.Should().Be(Guid.Empty);
        state.Data.Should().BeNull();
        state.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void RequestState_WithData_ShouldSetProperties()
    {
        var correlationId = Guid.NewGuid();
        var order = new OrderDto { Id = Guid.NewGuid(), UserId = "test-user" };

        var state = new OrderState
        {
            IsLoading = true,
            IsSaving = true,
            CorrelationId = correlationId,
            Data = order,
            ErrorMessage = "Test error",
        };

        state.IsLoading.Should().BeTrue();
        state.IsSaving.Should().BeTrue();
        state.CorrelationId.Should().Be(correlationId);
        state.Data.Should().Be(order);
        state.ErrorMessage.Should().Be("Test error");
    }

    [Fact]
    public void RequestState_RecordEquality_ShouldWork()
    {
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

        state1.Should().Be(state2);
    }

    [Fact]
    public void RequestState_RecordWith_ShouldCreateNewInstance()
    {
        var state = new OrderState
        {
            IsLoading = true,
            CorrelationId = Guid.NewGuid(),
        };

        var newState = state with { IsLoading = false };

        newState.IsLoading.Should().BeFalse();
        state.IsLoading.Should().BeTrue();
        newState.CorrelationId.Should().Be(state.CorrelationId);
    }

    [Fact]
    public void RequestState_ImplementsIRequestState()
    {
        var state = new OrderState();

        state.Should().BeAssignableTo<IRequestState<OrderDto>>();
    }
}
