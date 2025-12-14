using FluentAssertions;
using Lewee.StateManagement;
using Pizzeria.Store.Contracts.Pizzas;
using Pizzeria.Store.StateManagement.Pizzas.Actions;
using Xunit;

namespace Pizzeria.Store.StateManagement.Tests.Unit;

public class QuerySuccessActionTests
{
    [Fact]
    public void LoadPizzasSuccessAction_ShouldImplementIQuerySuccessAction()
    {
        // Arrange & Act
        var action = new LoadPizzasSuccessAction();

        // Assert
        action.Should().BeAssignableTo<IQuerySuccessAction<IEnumerable<PizzaDto>>>();
        action.Should().BeAssignableTo<IRequestSuccessAction>();
        action.Should().BeAssignableTo<IRequestAction>();
    }

    [Fact]
    public void LoadPizzasSuccessAction_ShouldSetCorrelationId()
    {
        // Arrange
        var correlationId = Guid.NewGuid();

        // Act
        var action = new LoadPizzasSuccessAction { CorrelationId = correlationId };

        // Assert
        action.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public void LoadPizzasSuccessAction_ShouldSetData()
    {
        // Arrange
        var pizzas = new List<PizzaDto>
        {
            new(Guid.NewGuid(), "Margherita", "Classic tomato and mozzarella", 12.99m),
            new(Guid.NewGuid(), "Pepperoni", "Topped with spicy pepperoni", 14.99m),
        };

        // Act
        var action = new LoadPizzasSuccessAction { Data = pizzas };

        // Assert
        action.Data.Should().BeEquivalentTo(pizzas);
    }

    [Fact]
    public void LoadPizzasSuccessAction_ShouldHaveEmptyDefaultData()
    {
        // Arrange & Act
        var action = new LoadPizzasSuccessAction();

        // Assert
        action.Data.Should().BeEmpty();
    }

    [Fact]
    public void LoadPizzasSuccessAction_ShouldSetAllProperties()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var pizzas = new List<PizzaDto>
        {
            new(Guid.NewGuid(), "Hawaiian", "Ham and pineapple", 15.99m),
        };

        // Act
        var action = new LoadPizzasSuccessAction
        {
            CorrelationId = correlationId,
            Data = pizzas,
        };

        // Assert
        action.CorrelationId.Should().Be(correlationId);
        action.Data.Should().ContainSingle();
        action.Data.First().Name.Should().Be("Hawaiian");
    }
}
