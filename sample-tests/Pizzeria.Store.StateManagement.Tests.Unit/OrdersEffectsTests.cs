using Bunit;
using Correlate;
using FluentAssertions;
using Fluxor;
using Lewee.Application.Mediation.Requests;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.Contracts.Orders.Actions;
using Pizzeria.Store.StateManagement.Orders;
using Xunit;

namespace Pizzeria.Store.StateManagement.Tests.Unit;

public class OrdersEffectsTests : TestContext
{
    private readonly Mock<IMediator> mediatorMock = new();
    private readonly Mock<IDispatcher> dispatcherMock = new();
    private readonly Mock<IState<OrdersState>> stateMock = new();
    private readonly OrdersEffects effects;

    public OrdersEffectsTests()
    {
        this.stateMock
            .Setup(s => s.Value)
            .Returns(new OrdersState());

        this.effects = new OrdersEffects(
            this.stateMock.Object,
            this.mediatorMock.Object,
            this.Services.GetRequiredService<NavigationManager>(),
            Mock.Of<ICorrelationContextAccessor>(),
            Mock.Of<ILogger<OrdersEffects>>());
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
        var action = new StartOrderCompletedAction(order, correlationId);

        // Act
        await this.effects.OnStartOrderCompletedAsync(action, this.dispatcherMock.Object);

        // Assert
        var navMan = this.Services.GetRequiredService<NavigationManager>();
        navMan.Uri.Should().Contain($"/order");
    }

    [Fact]
    public async Task ExecuteRequestAsync_Success_DispatchesSuccessActionAsync()
    {
        // Arrange
        this.mediatorMock
            .Setup(x => x.Send(It.IsAny<StartOrderCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CommandResult.Success());

        var correlationId = Guid.NewGuid();
        var action = new StartOrderAction { CorrelationId = correlationId };

        // Act
        await this.effects.RequestAsync(action, this.dispatcherMock.Object);

        // Assert
        this.dispatcherMock.Verify(
            d => d.Dispatch(It.Is<StartOrderSuccessAction>(a => a.CorrelationId == correlationId)),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteRequestAsync_Failure_DispatchesFailureActionAsync()
    {
        // Arrange
        var errorMessage = "Something bad happened";
        this.mediatorMock
            .Setup(x => x.Send(It.IsAny<StartOrderCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CommandResult.Fail(ResultStatus.BadRequest, errorMessage));

        var correlationId = Guid.NewGuid();
        var action = new StartOrderAction { CorrelationId = correlationId };

        // Act
        await this.effects.RequestAsync(action, this.dispatcherMock.Object);

        // Assert
        this.dispatcherMock.Verify(
            d => d.Dispatch(It.Is<StartOrderFailureAction>(a =>
                a.CorrelationId == correlationId &&
                a.ErrorMessage == errorMessage)),
            Times.Once);
    }
}
