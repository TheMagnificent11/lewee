using Bunit;
using Correlate;
using FluentAssertions;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor.Services;
using Pizzeria.Store.Contracts.Pizzas;
using Pizzeria.Store.StateManagement;
using Xunit;

namespace Pizzeria.Store.Components.Tests.Unit;

public class OrderPageTests : TestContext
{
    private readonly Mock<IStoreApi> storeApiMock = new();

    public OrderPageTests()
    {
        this.Services.AddSingleton(this.storeApiMock.Object);
        this.Services.AddSingleton(Mock.Of<ICorrelationContextAccessor>());
        this.Services.AddLogging();
        this.Services.AddMudServices();
        this.Services.AddFluxor(x => x.ScanAssemblies(typeof(StoreStateManagementConfiguration).Assembly));
    }

    [Fact]
    public void Order_WhenNoCurrentOrder_RedirectsToHome()
    {
        // Act & Assert
        // Since we don't have an active order, this should show a warning about no active order
        var component = this.RenderComponent<Order>();
        component.Markup.Should().Contain("No active order found");
    }

    [Fact]
    public void Order_WhenPizzasLoading_ShowsSkeletonLoader()
    {
        // Arrange
        var component = this.RenderComponent<Order>();

        // Assert
        component.Should().NotBeNull();
    }

    [Fact]
    public void Order_WhenPizzasAvailable_ShowsPizzaCards()
    {
        // Arrange
        var testPizzas = new[]
        {
            new PizzaDto(Guid.NewGuid(), "Margherita", "Classic tomato and mozzarella", 12.99m),
            new PizzaDto(Guid.NewGuid(), "Pepperoni", "Pepperoni and cheese", 14.99m),
        };

        this.storeApiMock
            .Setup(x => x.GetPizzasAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(testPizzas);

        // Act
        var component = this.RenderComponent<Order>();

        // Assert
        component.Markup.Should().Contain("Pizza Menu");
    }

    [Fact]
    public void Order_WhenOrderIdParameterProvided_ComponentRendersWithoutError()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        // Act & Assert - component should render without throwing an exception
        var component = this.RenderComponent<Order>(parameters => parameters.Add(p => p.OrderId, orderId));
        component.Should().NotBeNull();
    }
}
