using FluentAssertions;
using Lewee.Common;
using Lewee.Domain;
using Moq;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Domain;
using Xunit;

namespace Pizzeria.Store.Application.Tests.Unit;

public sealed class SetPizzaQuantityInOrderCommandTests
{
    [Fact]
    public async Task Should_SetPizzaQuantity_When_OrderExistsAsync()
    {
        var order = TestHelpers.CreateOrderWithPizza(out var pizza);
        var orderRepository = TestHelpers.Repository<Order>();
        orderRepository
            .Setup(item => item.QueryOneAsync(
                It.IsAny<QuerySpecification<Order>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        var pizzaRepository = TestHelpers.Repository<Pizza>();
        pizzaRepository
            .Setup(item => item.RetrieveByIdAsync(pizza.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pizza);
        var handler = new SetPizzaQuantityInOrderCommand.Handler(
            orderRepository.Object,
            pizzaRepository.Object,
            TestHelpers.CorrelationAccessor().Object,
            TestHelpers.Logger<SetPizzaQuantityInOrderCommand.Handler>());

        var result = await handler.Handle(
            new SetPizzaQuantityInOrderCommand(order.Id, pizza.Id, 4),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        order.Pizzas.Single().Quantity.Should().Be(4);
        orderRepository.Verify(item => item.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_RemovePizza_When_DesiredQuantityIsZeroAsync()
    {
        var order = TestHelpers.CreateOrderWithPizza(out var pizza);
        var handler = CreateHandler(order, pizza);

        var result = await handler.Handle(
            new SetPizzaQuantityInOrderCommand(order.Id, pizza.Id, 0),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        order.Pizzas.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_ReturnNotFound_When_PizzaDoesNotExistAsync()
    {
        var order = TestHelpers.CreateOrderWithPizza(out var pizza);
        var pizzaRepository = TestHelpers.Repository<Pizza>();
        pizzaRepository
            .Setup(item => item.RetrieveByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pizza?)null);
        var handler = new SetPizzaQuantityInOrderCommand.Handler(
            TestHelpers.Repository<Order>().Object,
            pizzaRepository.Object,
            TestHelpers.CorrelationAccessor().Object,
            TestHelpers.Logger<SetPizzaQuantityInOrderCommand.Handler>());

        var result = await handler.Handle(
            new SetPizzaQuantityInOrderCommand(order.Id, pizza.Id, 2),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    private static SetPizzaQuantityInOrderCommand.Handler CreateHandler(Order order, Pizza pizza)
    {
        var orderRepository = TestHelpers.Repository<Order>();
        orderRepository
            .Setup(item => item.QueryOneAsync(
                It.IsAny<QuerySpecification<Order>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        var pizzaRepository = TestHelpers.Repository<Pizza>();
        pizzaRepository
            .Setup(item => item.RetrieveByIdAsync(pizza.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pizza);

        return new SetPizzaQuantityInOrderCommand.Handler(
            orderRepository.Object,
            pizzaRepository.Object,
            TestHelpers.CorrelationAccessor().Object,
            TestHelpers.Logger<SetPizzaQuantityInOrderCommand.Handler>());
    }
}
