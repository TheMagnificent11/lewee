using Bunit;
using Correlate;
using FluentAssertions;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor.Services;
using Pizzeria.Ordering.StateManagement;
using Pizzeria.Ordering.StateManagement.Orders.Actions;
using Pizzeria.Store.Contracts.Orders;
using Xunit;

namespace Pizzeria.Ordering.Components.Tests.Unit;

public class CheckoutPageTests : TestContext
{
    private readonly Mock<IBffApiClient> bffApiClientMock = new();

    public CheckoutPageTests()
    {
        this.Services.AddSingleton(this.bffApiClientMock.Object);
        this.Services.AddSingleton(Mock.Of<ICorrelationContextAccessor>());
        this.Services.AddLogging();
        this.Services.AddMudServices();
        this.Services.AddFluxor(x => x.ScanAssemblies(typeof(StoreStateManagementConfiguration).Assembly));

        this.JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Checkout_WhenNoCurrentOrder_ShowsNoActiveOrderMessage()
    {
        // Act
        var component = this.RenderComponent<Checkout>();

        // Assert
        component.Markup.Should().Contain("No active order found");
    }

    [Fact]
    public async Task Checkout_WhenPickupSelected_DispatchesSubmitPickupOrder()
    {
        // Arrange
        var order = await this.SeedActiveOrderAsync(CreateInProgressOrder());

        this.bffApiClientMock
            .Setup(x => x.UpdateOrderAsync(order.Id, It.IsAny<UpdateOrderRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var component = this.RenderComponent<Checkout>();

        // Act
        var submitButton = component.Find(Checkout.Selectors.SubmitOrderButton);
        submitButton.Click();

        // Assert
        this.bffApiClientMock.Verify(
            x => x.UpdateOrderAsync(
                order.Id,
                It.Is<UpdateOrderRequest>(r => r.SubmitAsPickup),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Checkout_WhenDeliverySelectedWithValidAddress_DispatchesSubmitDeliveryOrder()
    {
        // Arrange
        var order = await this.SeedActiveOrderAsync(CreateInProgressOrder());

        this.bffApiClientMock
            .Setup(x => x.UpdateOrderAsync(order.Id, It.IsAny<UpdateOrderRequest>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var component = this.RenderComponent<Checkout>();

        var deliveryRadio = component.FindAll("input[type='radio']").Skip(1).First();
        deliveryRadio.Click();

        var addressInput = component.Find(Checkout.Selectors.DeliveryAddressInput);
        await addressInput.InputAsync(new ChangeEventArgs { Value = "123 Pizza Street" });

        // Act
        var submitButton = component.Find(Checkout.Selectors.SubmitOrderButton);
        submitButton.Click();

        // Assert
        this.bffApiClientMock.Verify(
            x => x.UpdateOrderAsync(
                order.Id,
                It.Is<UpdateOrderRequest>(r => r.SubmitAsDelivery && r.DeliveryAddress == "123 Pizza Street"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Checkout_WhenDeliverySelectedWithEmptyAddress_ShowsValidationError()
    {
        // Arrange
        await this.SeedActiveOrderAsync(CreateInProgressOrder());

        var component = this.RenderComponent<Checkout>();

        var deliveryRadio = component.FindAll("input[type='radio']").Skip(1).First();
        deliveryRadio.Click();

        // Act
        var submitButton = component.Find(Checkout.Selectors.SubmitOrderButton);
        submitButton.Click();

        // Assert
        component.Markup.Should().Contain("Delivery address is required.");
        this.bffApiClientMock.Verify(
            x => x.UpdateOrderAsync(It.IsAny<Guid>(), It.IsAny<UpdateOrderRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Checkout_WhenDeliveryAddressExceedsMaxLength_ShowsValidationError()
    {
        // Arrange
        await this.SeedActiveOrderAsync(CreateInProgressOrder());

        var component = this.RenderComponent<Checkout>();

        var deliveryRadio = component.FindAll("input[type='radio']").Skip(1).First();
        deliveryRadio.Click();

        var addressInput = component.Find(Checkout.Selectors.DeliveryAddressInput);
        await addressInput.InputAsync(new ChangeEventArgs { Value = new string('a', UpdateOrderRequest.DeliveryAddressMaxLength + 1) });

        // Act
        var submitButton = component.Find(Checkout.Selectors.SubmitOrderButton);
        submitButton.Click();

        // Assert
        component.Markup.Should().Contain(
            $"Delivery address must be no more than {UpdateOrderRequest.DeliveryAddressMaxLength} characters.");
        this.bffApiClientMock.Verify(
            x => x.UpdateOrderAsync(It.IsAny<Guid>(), It.IsAny<UpdateOrderRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Checkout_WhenOrderAlreadySubmitted_RedirectsToOrderPage()
    {
        // Arrange
        var order = CreateInProgressOrder() with { Status = OrderStatus.Received };
        await this.SeedActiveOrderAsync(order);

        // Act
        this.RenderComponent<Checkout>();

        // Assert
        var navigationManager = this.Services.GetRequiredService<NavigationManager>();
        navigationManager.Uri.Should().Contain($"/orders/{order.Id}");
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

    private async Task<OrderDto> SeedActiveOrderAsync(OrderDto order)
    {
        var store = this.Services.GetRequiredService<IStore>();
        await store.InitializeAsync();

        var dispatcher = this.Services.GetRequiredService<IDispatcher>();
        dispatcher.Dispatch(new StartOrderCompletedAction { Data = order, CorrelationId = Guid.NewGuid() });

        return order;
    }
}
