#nullable enable
using Bunit;
using Correlate;
using FluentAssertions;
using Fluxor;
using Lewee.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.StateManagement.Orders;
using Pizzeria.Store.StateManagement.Orders.Actions;
using Xunit;

namespace Pizzeria.Store.StateManagement.Tests.Unit;

public class OrdersEffectsTests : TestContext
{
    private readonly Mock<IStoreApiClient> storeApiClientMock = new();
    private readonly Mock<IDispatcher> dispatcherMock = new();
    private readonly Mock<IState<OrderState>> stateMock = new();
    private readonly StartOrderEffects effects;

    public OrdersEffectsTests()
    {
        this.stateMock
            .Setup(s => s.Value)
            .Returns(new OrderState());

        this.effects = new StartOrderEffects(
            this.stateMock.Object,
            this.storeApiClientMock.Object,
            this.Services.GetRequiredService<NavigationManager>(),
            Mock.Of<ICorrelationContextAccessor>(),
            Mock.Of<ILogger<StartOrderEffects>>());
    }

    [Fact]
    public async Task OnStartOrderCompletedAsync_NavigatesToOrderPageAsync()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var order = new OrderDto
        {
            Id = orderId,
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
        await this.effects.OnCommandCompletedAsync(action, this.dispatcherMock.Object);

        // Assert
        var navMan = this.Services.GetRequiredService<NavigationManager>();
        navMan.Uri.Should().Contain($"/order");
    }

    [Fact]
    public async Task OnStartOrderCompletedAsync_WithNullOrder_DoesNotNavigateAsync()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var action = new StartOrderCompletedAction
        {
            Data = new OrderDto(),
            CorrelationId = correlationId,
        };
        var navMan = this.Services.GetRequiredService<NavigationManager>();
        var initialUri = navMan.Uri;

        // Act
        await this.effects.OnCommandCompletedAsync(action, this.dispatcherMock.Object);

        // Assert
        navMan.Uri.Should().Be(initialUri);
    }

    [Fact]
    public async Task OnStartOrderCompletedAsync_WithEmptyOrderId_DoesNotNavigateAsync()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var order = new OrderDto
        {
            Id = Guid.Empty,
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
        var navMan = this.Services.GetRequiredService<NavigationManager>();
        var initialUri = navMan.Uri;

        // Act
        await this.effects.OnCommandCompletedAsync(action, this.dispatcherMock.Object);

        // Assert
        navMan.Uri.Should().Be(initialUri);
    }

    [Fact]
    public async Task ExecuteRequestAsync_Success_DispatchesSuccessActionAsync()
    {
        // Arrange
        this.storeApiClientMock
            .Setup(x => x.StartOrderAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var correlationId = Guid.NewGuid();
        var action = new StartOrderAction { CorrelationId = correlationId };

        // Act
        await this.effects.OnCommandAsync(action, this.dispatcherMock.Object);

        // Assert
        this.dispatcherMock.Verify(
            d => d.Dispatch(It.Is<StartOrderSuccessAction>(a => a.CorrelationId == correlationId)),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteRequestAsync_Exception_DispatchesFailureActionAsync()
    {
        // Arrange
        var exceptionMessage = "Unexpected error";
        this.storeApiClientMock
            .Setup(x => x.StartOrderAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException(exceptionMessage));

        var correlationId = Guid.NewGuid();
        var action = new StartOrderAction { CorrelationId = correlationId };

        // Act
        await this.effects.OnCommandAsync(action, this.dispatcherMock.Object);

        // Assert
        this.dispatcherMock.Verify(
            d => d.Dispatch(It.Is<StartOrderFailureAction>(
                a => a.CorrelationId == correlationId && a.ErrorMessage == exceptionMessage)),
            Times.Once);
    }
}
