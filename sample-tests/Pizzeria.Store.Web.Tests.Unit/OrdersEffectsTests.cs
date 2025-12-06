using Bunit;
using Correlate;
using FluentAssertions;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pizzeria.Store.Contracts;
using Pizzeria.Store.Web.Infrastructure;
using Pizzeria.Store.Web.Orders;
using Pizzeria.Store.Web.Orders.Actions;

namespace Pizzeria.Store.Web.Tests.Unit;

public class OrdersEffectsTests : TestContext
{
    [Fact]
    public async Task OnStartOrderCompletedAsync_NavigatesToOrderPageAsync()
    {
        // Arrange
        var mockState = new Mock<IState<OrdersState>>();
        mockState.Setup(s => s.Value).Returns(new OrdersState());

        var mockCorrelationContextAccessor = new Mock<ICorrelationContextAccessor>();
        var mockLogger = new Mock<ILogger<OrdersEffects>>();
        var mockApiClient = new Mock<IPizzeriaApiClient>();
        var mockDispatcher = new Mock<IDispatcher>();

        var effects = new OrdersEffects(
            mockState.Object,
            mockApiClient.Object,
            this.Services.GetRequiredService<NavigationManager>(),
            mockCorrelationContextAccessor.Object,
            mockLogger.Object);

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
        var action = new StartOrderCompletedAction(order, correlationId);

        // Act
        await effects.OnStartOrderCompletedAsync(action, mockDispatcher.Object);

        // Assert
        var navMan = this.Services.GetRequiredService<NavigationManager>();
        navMan.Uri.Should().Contain($"/order");
    }

    [Fact]
    public async Task ExecuteRequestAsync_Success_DispatchesSuccessActionAsync()
    {
        // Arrange
        var mockState = new Mock<IState<OrdersState>>();
        mockState.Setup(s => s.Value).Returns(new OrdersState());

        var mockCorrelationContextAccessor = new Mock<ICorrelationContextAccessor>();
        var mockLogger = new Mock<ILogger<OrdersEffects>>();
        var mockApiClient = new Mock<IPizzeriaApiClient>();
        mockApiClient.Setup(c => c.StartOrderAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var mockDispatcher = new Mock<IDispatcher>();

        var effects = new OrdersEffects(
            mockState.Object,
            mockApiClient.Object,
            this.Services.GetRequiredService<NavigationManager>(),
            mockCorrelationContextAccessor.Object,
            mockLogger.Object);

        var correlationId = Guid.NewGuid();
        var action = new StartOrderAction { CorrelationId = correlationId };

        // Act
        await effects.RequestAsync(action, mockDispatcher.Object);

        // Assert
        mockDispatcher.Verify(
            d => d.Dispatch(It.Is<StartOrderSuccessAction>(a => a.CorrelationId == correlationId)),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteRequestAsync_Failure_DispatchesFailureActionAsync()
    {
        // Arrange
        var mockState = new Mock<IState<OrdersState>>();
        mockState.Setup(s => s.Value).Returns(new OrdersState());

        var mockCorrelationContextAccessor = new Mock<ICorrelationContextAccessor>();
        var mockLogger = new Mock<ILogger<OrdersEffects>>();
        var mockApiClient = new Mock<IPizzeriaApiClient>();
        mockApiClient.Setup(c => c.StartOrderAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("API error"));

        var mockDispatcher = new Mock<IDispatcher>();

        var effects = new OrdersEffects(
            mockState.Object,
            mockApiClient.Object,
            this.Services.GetRequiredService<NavigationManager>(),
            mockCorrelationContextAccessor.Object,
            mockLogger.Object);

        var correlationId = Guid.NewGuid();
        var action = new StartOrderAction { CorrelationId = correlationId };

        // Act
        await effects.RequestAsync(action, mockDispatcher.Object);

        // Assert
        mockDispatcher.Verify(
            d => d.Dispatch(It.Is<StartOrderFailureAction>(a =>
                a.CorrelationId == correlationId &&
                a.ErrorMessage.Contains("API error", StringComparison.Ordinal))),
            Times.Once);
    }
}
