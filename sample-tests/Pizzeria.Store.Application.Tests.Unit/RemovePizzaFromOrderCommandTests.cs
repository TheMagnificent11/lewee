using FluentAssertions;
using Lewee.Common;
using Lewee.Domain;
using Moq;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Domain;
using Xunit;

namespace Pizzeria.Store.Application.Tests.Unit;

public sealed class RemovePizzaFromOrderCommandTests
{
    [Fact]
    public async Task Should_RemovePizza_When_OrderOwnedByCallerAsync()
    {
        var order = TestHelpers.CreateOrderWithPizza(out var pizza);
        order.AddPizza(pizza);

        var handler = CreateHandler(order, pizza, TestHelpers.OwnerUserId);

        var result = await handler.Handle(
            new RemovePizzaFromOrderCommand(order.Id, pizza.Id),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        order.Pizzas.Single(x => x.PizzaId == pizza.Id).Quantity.Should().Be(1);
    }

    [Fact]
    public async Task Should_Fail_When_PizzaNotFoundAsync()
    {
        var order = TestHelpers.CreateOrderWithPizza(out var pizza);

        var orderRepository = TestHelpers.Repository<Order>();
        var pizzaRepository = TestHelpers.Repository<Pizza>();
        pizzaRepository
            .Setup(x => x.RetrieveByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pizza?)null);

        var handler = new RemovePizzaFromOrderCommand.Handler(
            orderRepository.Object,
            pizzaRepository.Object,
            TestHelpers.AuthenticatedUser(TestHelpers.OwnerUserId).Object,
            TestHelpers.Logger<RemovePizzaFromOrderCommand.Handler>());

        var result = await handler.Handle(
            new RemovePizzaFromOrderCommand(order.Id, pizza.Id),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Should_Fail_When_OrderOwnedByDifferentCallerAsync()
    {
        var order = TestHelpers.CreateOrderWithPizza(out var pizza);

        var handler = CreateHandler(order, pizza, TestHelpers.OtherUserId);

        var result = await handler.Handle(
            new RemovePizzaFromOrderCommand(order.Id, pizza.Id),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Unauthorized);
    }

    private static RemovePizzaFromOrderCommand.Handler CreateHandler(Order order, Pizza pizza, string callerUserId)
    {
        var orderRepository = TestHelpers.Repository<Order>();
        orderRepository
            .Setup(x => x.QueryOneAsync(It.IsAny<QuerySpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var pizzaRepository = TestHelpers.Repository<Pizza>();
        pizzaRepository
            .Setup(x => x.RetrieveByIdAsync(pizza.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pizza);

        return new RemovePizzaFromOrderCommand.Handler(
            orderRepository.Object,
            pizzaRepository.Object,
            TestHelpers.AuthenticatedUser(callerUserId).Object,
            TestHelpers.Logger<RemovePizzaFromOrderCommand.Handler>());
    }
}
