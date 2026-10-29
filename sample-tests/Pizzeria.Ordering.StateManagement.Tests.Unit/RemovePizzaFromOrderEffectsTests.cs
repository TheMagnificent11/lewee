using Fluxor;
using Microsoft.Extensions.Logging;
using Moq;
using Pizzeria.Ordering.StateManagement.Orders;
using Pizzeria.Ordering.StateManagement.Orders.Actions;
using Pizzeria.Store.Contracts.Orders;
using Xunit;

namespace Pizzeria.Ordering.StateManagement.Tests.Unit;

public class RemovePizzaFromOrderEffectsTests
{
    private readonly Mock<IBffApiClient> bffApiClientMock = new();
    private readonly Mock<IDispatcher> dispatcherMock = new();
    private readonly Mock<IState<OrderState>> stateMock = new();
    private readonly RemovePizzaFromOrderEffects effects;

    public RemovePizzaFromOrderEffectsTests()
    {
        this.stateMock
            .Setup(s => s.Value)
            .Returns(new OrderState());

        this.effects = new RemovePizzaFromOrderEffects(
            this.stateMock.Object,
            this.bffApiClientMock.Object,
            Mock.Of<Correlate.ICorrelationContextAccessor>(),
            Mock.Of<ILogger<RemovePizzaFromOrderEffects>>());
    }

    [Fact]
    public async Task ExecuteRequestAsync_Success_DispatchesSuccessActionAsync()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var pizzaId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        this.bffApiClientMock
            .Setup(x => x.SetOrderPizzaQuantityAsync(
                orderId,
                pizzaId,
                It.Is<SetPizzaQuantityRequest>(r => r.Quantity == 2),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var order = new OrderDto { Id = orderId };
        this.bffApiClientMock
            .Setup(x => x.GetOrderAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var action = new RemovePizzaFromOrderAction
        {
            OrderId = orderId,
            PizzaId = pizzaId,
            Quantity = 2,
            CorrelationId = correlationId,
        };

        // Act
        await this.effects.OnCommandAsync(action, this.dispatcherMock.Object);

        // Assert
        this.dispatcherMock.Verify(
            d => d.Dispatch(It.Is<RemovePizzaFromOrderSuccessAction>(a => a.CorrelationId == correlationId)),
            Times.Once);
        this.dispatcherMock.Verify(
            d => d.Dispatch(It.Is<OrderCommandReconciledAction>(a =>
                a.Data == order && a.CorrelationId == correlationId)),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteRequestAsync_QuantityBecomesZero_DeletesOrderPizzaAsync()
    {
        var orderId = Guid.NewGuid();
        var pizzaId = Guid.NewGuid();
        var action = new RemovePizzaFromOrderAction
        {
            OrderId = orderId,
            PizzaId = pizzaId,
            Quantity = 0,
        };
        this.bffApiClientMock
            .Setup(x => x.GetOrderAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OrderDto { Id = orderId });

        await this.effects.OnCommandAsync(action, this.dispatcherMock.Object);

        this.bffApiClientMock.Verify(
            client => client.RemovePizzaFromOrderAsync(orderId, pizzaId, It.IsAny<CancellationToken>()),
            Times.Once);
        this.bffApiClientMock.Verify(
            client => client.GetOrderAsync(orderId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteRequestAsync_Exception_DispatchesFailureActionAsync()
    {
        // Arrange
        var exceptionMessage = "Unexpected error";
        this.bffApiClientMock
            .Setup(x => x.SetOrderPizzaQuantityAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<SetPizzaQuantityRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException(exceptionMessage));

        var correlationId = Guid.NewGuid();
        var action = new RemovePizzaFromOrderAction
        {
            OrderId = Guid.NewGuid(),
            PizzaId = Guid.NewGuid(),
            Quantity = 1,
            CorrelationId = correlationId,
        };

        // Act
        await this.effects.OnCommandAsync(action, this.dispatcherMock.Object);

        // Assert
        this.dispatcherMock.Verify(
            d => d.Dispatch(It.Is<RemovePizzaFromOrderFailureAction>(
                a => a.CorrelationId == correlationId && a.ErrorMessage == exceptionMessage)),
            Times.Once);
    }
}
