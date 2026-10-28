using Fluxor;
using Microsoft.Extensions.Logging;
using Moq;
using Pizzeria.Ordering.StateManagement.Orders;
using Pizzeria.Ordering.StateManagement.Orders.Actions;
using Pizzeria.Store.Contracts.Orders;
using Xunit;

namespace Pizzeria.Ordering.StateManagement.Tests.Unit;

public class SubmitDeliveryOrderEffectsTests
{
    private readonly Mock<IBffApiClient> bffApiClientMock = new();
    private readonly Mock<IDispatcher> dispatcherMock = new();
    private readonly Mock<IState<OrderState>> stateMock = new();
    private readonly SubmitDeliveryOrderEffects effects;

    public SubmitDeliveryOrderEffectsTests()
    {
        this.stateMock
            .Setup(s => s.Value)
            .Returns(new OrderState());

        this.effects = new SubmitDeliveryOrderEffects(
            this.stateMock.Object,
            this.bffApiClientMock.Object,
            Mock.Of<Correlate.ICorrelationContextAccessor>(),
            Mock.Of<ILogger<SubmitDeliveryOrderEffects>>());
    }

    [Fact]
    public async Task ExecuteRequestAsync_Success_DispatchesSuccessActionAsync()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var deliveryAddress = "123 Main Street";

        this.bffApiClientMock
            .Setup(x => x.UpdateOrderAsync(
                orderId,
                It.Is<UpdateOrderRequest>(r => r.SubmitAsDelivery && r.DeliveryAddress == deliveryAddress),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var action = new SubmitDeliveryOrderAction
        {
            OrderId = orderId,
            DeliveryAddress = deliveryAddress,
            CorrelationId = correlationId,
        };

        // Act
        await this.effects.OnCommandAsync(action, this.dispatcherMock.Object);

        // Assert
        this.dispatcherMock.Verify(
            d => d.Dispatch(It.Is<SubmitDeliveryOrderSuccessAction>(a => a.CorrelationId == correlationId)),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteRequestAsync_Exception_DispatchesFailureActionAsync()
    {
        // Arrange
        var exceptionMessage = "Unexpected error";
        this.bffApiClientMock
            .Setup(x => x.UpdateOrderAsync(
                It.IsAny<Guid>(),
                It.IsAny<UpdateOrderRequest>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException(exceptionMessage));

        var correlationId = Guid.NewGuid();
        var action = new SubmitDeliveryOrderAction
        {
            OrderId = Guid.NewGuid(),
            DeliveryAddress = "123 Main Street",
            CorrelationId = correlationId,
        };

        // Act
        await this.effects.OnCommandAsync(action, this.dispatcherMock.Object);

        // Assert
        this.dispatcherMock.Verify(
            d => d.Dispatch(It.Is<SubmitDeliveryOrderFailureAction>(
                a => a.CorrelationId == correlationId && a.ErrorMessage == exceptionMessage)),
            Times.Once);
    }
}
