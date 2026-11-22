using Fluxor;
using Microsoft.Extensions.Logging;
using Moq;
using Pizzeria.Ordering.StateManagement.Orders;
using Pizzeria.Ordering.StateManagement.Orders.Actions;
using Pizzeria.Store.Contracts.Orders;
using Xunit;

namespace Pizzeria.Ordering.StateManagement.Tests.Unit;

public sealed class RefreshOrderEffectsTests
{
    [Fact]
    public async Task RefreshOrderAsync_DispatchesUpdatedOrder()
    {
        var orderId = Guid.NewGuid();
        var order = new OrderDto { Id = orderId, Revision = 2 };
        var bffApiClient = new Mock<IBffApiClient>();
        var cancellationTokenProvider = new Mock<IOrderReconciliationCancellationTokenProvider>();
        var dispatcher = new Mock<IDispatcher>();
        bffApiClient
            .Setup(client => client.GetOrderAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        var effects = new RefreshOrderEffects(
            bffApiClient.Object,
            cancellationTokenProvider.Object,
            Mock.Of<ILogger<RefreshOrderEffects>>());

        await effects.RefreshOrderAsync(new RefreshOrderAction { OrderId = orderId }, dispatcher.Object);

        dispatcher.Verify(
            item => item.Dispatch(It.Is<OrderUpdatedAction>(action => action.Data == order)),
            Times.Once);
    }

    [Fact]
    public async Task RefreshOrderAsync_WhenRequestFailsDoesNotDispatchOrderUpdate()
    {
        var orderId = Guid.NewGuid();
        var bffApiClient = new Mock<IBffApiClient>();
        var cancellationTokenProvider = new Mock<IOrderReconciliationCancellationTokenProvider>();
        var dispatcher = new Mock<IDispatcher>();
        bffApiClient
            .Setup(client => client.GetOrderAsync(orderId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException());
        var effects = new RefreshOrderEffects(
            bffApiClient.Object,
            cancellationTokenProvider.Object,
            Mock.Of<ILogger<RefreshOrderEffects>>());

        await effects.RefreshOrderAsync(new RefreshOrderAction { OrderId = orderId }, dispatcher.Object);

        dispatcher.Verify(item => item.Dispatch(It.IsAny<OrderUpdatedAction>()), Times.Never);
    }
}
