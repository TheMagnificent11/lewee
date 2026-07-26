using FluentAssertions;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.Contracts.Pizzas;
using Pizzeria.Store.StateManagement.Orders;
using Pizzeria.Store.StateManagement.Orders.Actions;
using Pizzeria.Store.StateManagement.Pizzas;
using Pizzeria.Store.StateManagement.Pizzas.Actions;
using Xunit;

namespace Lewee.Infrastructure.Fluxor.Tests.Unit;

public class ReducerExtensionsTests
{
    [Fact]
    public void OnCommand_WithClearData_SetsIsSavingAndClearsData()
    {
        var state = new OrderState { Data = new OrderDto { Id = Guid.NewGuid(), UserId = "test" } };
        var correlationId = Guid.NewGuid();
        var action = new StartOrderAction { CorrelationId = correlationId };

        var result = state.OnCommand<OrderState, OrderDto, StartOrderAction>(action, clearData: true);

        result.IsSaving.Should().BeTrue();
        result.CorrelationId.Should().Be(correlationId);
        result.ErrorMessage.Should().BeNull();
        result.Data.Should().BeNull();
    }

    [Fact]
    public void OnCommand_WithoutClearData_SetsIsSavingAndKeepsData()
    {
        var existingData = new OrderDto { Id = Guid.NewGuid(), UserId = "test" };
        var state = new OrderState { Data = existingData };
        var correlationId = Guid.NewGuid();
        var action = new StartOrderAction { CorrelationId = correlationId };

        var result = state.OnCommand<OrderState, OrderDto, StartOrderAction>(action, clearData: false);

        result.IsSaving.Should().BeTrue();
        result.CorrelationId.Should().Be(correlationId);
        result.ErrorMessage.Should().BeNull();
        result.Data.Should().Be(existingData);
    }

    [Fact]
    public void OnQuery_SetsIsLoadingAndClearsData()
    {
        var state = new PizzasState { Data = [new PizzaDto(Guid.NewGuid(), "Test", "Test", 10m)] };
        var correlationId = Guid.NewGuid();
        var action = new LoadPizzasAction { CorrelationId = correlationId };

        var result = state.OnQuery<PizzasState, IEnumerable<PizzaDto>, LoadPizzasAction>(action);

        result.IsLoading.Should().BeTrue();
        result.CorrelationId.Should().Be(correlationId);
        result.Data.Should().BeNull();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void OnCommandSuccess_ClearsIsSavingAndError()
    {
        var state = new OrderState { IsSaving = true, ErrorMessage = "Previous error" };
        var correlationId = Guid.NewGuid();
        var action = new StartOrderSuccessAction { CorrelationId = correlationId };

        var result = state.OnCommandSuccess<OrderState, OrderDto, StartOrderSuccessAction>(action);

        result.IsSaving.Should().BeFalse();
        result.CorrelationId.Should().Be(correlationId);
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void OnQuerySuccess_SetsDataAndClearsIsLoading()
    {
        var state = new PizzasState { IsLoading = true };
        var correlationId = Guid.NewGuid();
        var pizzas = new List<PizzaDto> { new(Guid.NewGuid(), "Margherita", "Classic", 12.99m) };
        var action = new LoadPizzasSuccessAction { CorrelationId = correlationId, Data = pizzas };

        var result = state.OnQuerySuccess<PizzasState, IEnumerable<PizzaDto>, LoadPizzasSuccessAction>(action);

        result.IsLoading.Should().BeFalse();
        result.CorrelationId.Should().Be(correlationId);
        result.Data.Should().BeEquivalentTo(pizzas);
    }

    [Fact]
    public void OnCommandError_SetsErrorMessageAndClearsIsSaving()
    {
        var state = new OrderState { IsSaving = true };
        var correlationId = Guid.NewGuid();
        var errorMessage = "Command failed";
        var action = new StartOrderFailureAction { CorrelationId = correlationId, ErrorMessage = errorMessage };

        var result = state.OnCommandError<OrderState, OrderDto, StartOrderFailureAction>(action);

        result.IsSaving.Should().BeFalse();
        result.CorrelationId.Should().Be(correlationId);
        result.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public void OnQueryError_SetsErrorMessageAndClearsIsLoading()
    {
        var state = new PizzasState { IsLoading = true };
        var correlationId = Guid.NewGuid();
        var errorMessage = "Query failed";
        var action = new LoadPizzasFailureAction { CorrelationId = correlationId, ErrorMessage = errorMessage };

        var result = state.OnQueryError<PizzasState, IEnumerable<PizzaDto>, LoadPizzasFailureAction>(action);

        result.IsLoading.Should().BeFalse();
        result.CorrelationId.Should().Be(correlationId);
        result.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public void OnCommandCompleted_SetsData()
    {
        var state = new OrderState();
        var correlationId = Guid.NewGuid();
        var order = new OrderDto { Id = Guid.NewGuid(), UserId = "test" };
        var action = new StartOrderCompletedAction { CorrelationId = correlationId, Data = order };

        var result = state.OnCommandCompleted<OrderState, OrderDto, StartOrderCompletedAction>(action);

        result.CorrelationId.Should().Be(correlationId);
        result.Data.Should().Be(order);
    }
}
