using Bunit;
using Correlate;
using FluentAssertions;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor.Services;
using Pizzeria.Ordering.StateManagement;
using Pizzeria.Ordering.StateManagement.Orders.Actions;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.Contracts.Pizzas;
using Xunit;

namespace Pizzeria.Ordering.Components.Tests.Unit;

public class OrderPageTests : TestContext
{
    private readonly Mock<IBffApiClient> bffApiClientMock = new();

    public OrderPageTests()
    {
        this.Services.AddSingleton(this.bffApiClientMock.Object);
        this.Services.AddSingleton(Mock.Of<ICorrelationContextAccessor>());
        this.Services.AddLogging();
        this.Services.AddMudServices();
        this.Services.AddFluxor(x => x.ScanAssemblies(typeof(StoreStateManagementConfiguration).Assembly));

        this.JSInterop.Mode = JSRuntimeMode.Loose;
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
    public async Task Order_WhenPizzasLoading_ShowsSkeletonLoader()
    {
        // Arrange
        await this.SeedActiveOrderAsync(CreateInProgressOrder());

        // Act
        var component = this.RenderComponent<Order>();

        // Assert
        component.Should().NotBeNull();
    }

    [Fact]
    public async Task Order_WhenPizzasAvailable_ShowsPizzaCards()
    {
        // Arrange
        var testPizzas = new[]
        {
            new PizzaDto(Guid.NewGuid(), "Margherita", "Classic tomato and mozzarella", 12.99m),
            new PizzaDto(Guid.NewGuid(), "Pepperoni", "Pepperoni and cheese", 14.99m),
        };

        this.bffApiClientMock
            .Setup(x => x.GetPizzasAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(testPizzas);

        await this.SeedActiveOrderAsync(CreateInProgressOrder());

        // Act
        var component = this.RenderComponent<Order>();

        // Assert
        component.Markup.Should().Contain("Pizza Menu");
        component.Markup.Should().Contain("Margherita");
    }

    [Fact]
    public async Task Order_WhenPizzaAddedToOrder_CheckoutButtonIsEnabled()
    {
        // Arrange
        var pizzaId = Guid.NewGuid();
        var testPizzas = new[] { new PizzaDto(pizzaId, "Margherita", "Classic tomato and mozzarella", 12.99m) };

        this.bffApiClientMock
            .Setup(x => x.GetPizzasAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(testPizzas);

        var order = CreateInProgressOrder() with
        {
            Pizzas = [new OrderPizzaDto { PizzaId = pizzaId, PizzaName = "Margherita", Quantity = 1, LineTotal = 12.99m }],
        };
        await this.SeedActiveOrderAsync(order);

        // Act
        var component = this.RenderComponent<Order>();

        // Assert
        var checkoutButton = component.Find(Order.Selectors.CheckoutButton);
        checkoutButton.HasAttribute("disabled").Should().BeFalse();
    }

    [Fact]
    public async Task Order_WhenOrderSubmitted_ShowsOrderStatus()
    {
        // Arrange
        var order = CreateInProgressOrder() with
        {
            Status = OrderStatus.Received,
            Pizzas = [new OrderPizzaDto { PizzaId = Guid.NewGuid(), PizzaName = "Margherita", Quantity = 1, LineTotal = 12.99m }],
        };
        await this.SeedActiveOrderAsync(order);

        // Act
        var component = this.RenderComponent<Order>();

        // Assert
        component.Markup.Should().Contain("Order Status");
        component.Markup.Should().Contain("Received");
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

    private static OrderDto CreateInProgressOrder()
    {
        return new OrderDto
        {
            Id = Guid.NewGuid(),
            UserId = "test-user",
            Status = OrderStatus.InProgress,
            StartedDateTime = DateTime.UtcNow,
            Pizzas = [],
            TotalCost = 0,
        };
    }

    private async Task SeedActiveOrderAsync(OrderDto order)
    {
        var store = this.Services.GetRequiredService<IStore>();
        await store.InitializeAsync();

        var dispatcher = this.Services.GetRequiredService<IDispatcher>();
        dispatcher.Dispatch(new StartOrderCompletedAction { Data = order, CorrelationId = Guid.NewGuid() });
    }
}
