using FluentAssertions;
using Pizzeria.Store.Contracts.Pizzas;
using Pizzeria.Store.StateManagement.Pizzas;
using Pizzeria.Store.StateManagement.Pizzas.Actions;
using Xunit;

namespace Pizzeria.Store.StateManagement.Tests.Unit;

public class PizzasReducerTests
{
    [Fact]
    public void OnLoadPizzas_SetsIsLoadingToTrue()
    {
        // Arrange
        var state = new PizzasState();
        var action = new LoadPizzasAction();

        // Act
        var result = PizzasReducer.OnLoadPizzas(state, action);

        // Assert
        result.IsLoading.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void OnLoadPizzas_ClearsExistingErrorMessage()
    {
        // Arrange
        var state = new PizzasState { ErrorMessage = "Previous error" };
        var action = new LoadPizzasAction();

        // Act
        var result = PizzasReducer.OnLoadPizzas(state, action);

        // Assert
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void OnLoadPizzasSuccess_SetsPizzas()
    {
        // Arrange
        var state = new PizzasState { IsLoading = true };
        var pizzas = new List<PizzaDto>
        {
            new(Guid.NewGuid(), "Margherita", "Classic pizza", 9.99m),
            new(Guid.NewGuid(), "Pepperoni", "Spicy pizza", 11.99m),
        };
        var action = new LoadPizzasSuccessAction(pizzas);

        // Act
        var result = PizzasReducer.OnLoadPizzasSuccess(state, action);

        // Assert
        result.Pizzas.Should().BeEquivalentTo(pizzas);
        result.IsLoading.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void OnLoadPizzasSuccess_ClearsExistingErrorMessage()
    {
        // Arrange
        var state = new PizzasState { IsLoading = true, ErrorMessage = "Previous error" };
        var pizzas = new List<PizzaDto>
        {
            new(Guid.NewGuid(), "Margherita", "Classic pizza", 9.99m),
        };
        var action = new LoadPizzasSuccessAction(pizzas);

        // Act
        var result = PizzasReducer.OnLoadPizzasSuccess(state, action);

        // Assert
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public void OnLoadPizzasFailure_SetsErrorMessage()
    {
        // Arrange
        var state = new PizzasState { IsLoading = true };
        var errorMessage = "Failed to load pizzas";
        var action = new LoadPizzasFailureAction(errorMessage);

        // Act
        var result = PizzasReducer.OnLoadPizzasFailure(state, action);

        // Assert
        result.IsLoading.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
    }

    [Fact]
    public void OnLoadPizzasFailure_PreservesPizzas()
    {
        // Arrange
        var existingPizzas = new List<PizzaDto>
        {
            new(Guid.NewGuid(), "Margherita", "Classic pizza", 9.99m),
        };
        var state = new PizzasState { Pizzas = existingPizzas, IsLoading = true };
        var action = new LoadPizzasFailureAction("Failed to load pizzas");

        // Act
        var result = PizzasReducer.OnLoadPizzasFailure(state, action);

        // Assert
        result.Pizzas.Should().BeEquivalentTo(existingPizzas);
    }
}
