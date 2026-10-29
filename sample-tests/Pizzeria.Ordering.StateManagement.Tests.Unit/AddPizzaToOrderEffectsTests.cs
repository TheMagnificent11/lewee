using Fluxor;
using Microsoft.Extensions.Logging;
using Moq;
using Pizzeria.Ordering.StateManagement.Orders;
using Pizzeria.Ordering.StateManagement.Orders.Actions;
using Pizzeria.Store.Contracts.Orders;
using Xunit;

namespace Pizzeria.Ordering.StateManagement.Tests.Unit;

public sealed class AddPizzaToOrderEffectsTests
{
    [Fact]
    public async Task Should_SetDesiredQuantity_When_AddingPizzaAsync()
    {
        var orderId = Guid.NewGuid();
        var pizzaId = Guid.NewGuid();
        var client = new Mock<IBffApiClient>();
        client
            .Setup(item => item.SetOrderPizzaQuantityAsync(
                orderId,
                pizzaId,
                It.Is<SetPizzaQuantityRequest>(request => request.Quantity == 3),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var order = new OrderDto { Id = orderId };
        client
            .Setup(item => item.GetOrderAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        var dispatcher = new Mock<IDispatcher>();
        var effect = new AddPizzaToOrderEffects(
            Mock.Of<IState<OrderState>>(),
            client.Object,
            Mock.Of<Correlate.ICorrelationContextAccessor>(),
            Mock.Of<ILogger<AddPizzaToOrderEffects>>());

        await effect.OnCommandAsync(
            new AddPizzaToOrderAction { OrderId = orderId, PizzaId = pizzaId, Quantity = 3 },
            dispatcher.Object);

        client.Verify(
            item => item.SetOrderPizzaQuantityAsync(
                orderId,
                pizzaId,
                It.Is<SetPizzaQuantityRequest>(request => request.Quantity == 3),
                It.IsAny<CancellationToken>()),
            Times.Once);
        dispatcher.Verify(
            item => item.Dispatch(It.Is<OrderCommandReconciledAction>(action =>
                action.Data == order && action.CorrelationId != Guid.Empty)),
            Times.Once);
    }
}
