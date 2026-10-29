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
        var effect = new AddPizzaToOrderEffects(
            Mock.Of<IState<OrderState>>(),
            client.Object,
            Mock.Of<Correlate.ICorrelationContextAccessor>(),
            Mock.Of<ILogger<AddPizzaToOrderEffects>>());

        await effect.OnCommandAsync(
            new AddPizzaToOrderAction { OrderId = orderId, PizzaId = pizzaId, Quantity = 3 },
            Mock.Of<IDispatcher>());

        client.Verify(
            item => item.SetOrderPizzaQuantityAsync(
                orderId,
                pizzaId,
                It.Is<SetPizzaQuantityRequest>(request => request.Quantity == 3),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
