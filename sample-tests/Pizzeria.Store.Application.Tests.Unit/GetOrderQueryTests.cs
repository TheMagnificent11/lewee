using FluentAssertions;
using Lewee.Common;
using Lewee.Domain;
using Moq;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.Domain;
using Xunit;

namespace Pizzeria.Store.Application.Tests.Unit;

public sealed class GetOrderQueryTests
{
    [Fact]
    public async Task Should_ReturnOrderWithPizzasAndTotalAsync()
    {
        var order = TestHelpers.CreateOrderWithPizza(out var pizza);
        order.AddPizza(pizza);

        var handler = CreateHandler(order);

        var result = await handler.Handle(new GetOrderQuery(order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Pizzas.Should().ContainSingle();
        result.Data.Pizzas[0].Quantity.Should().Be(2);
        result.Data.TotalCost.Should().Be(pizza.Price * 2);
        result.Data.Status.Should().Be(OrderStatus.InProgress);
    }

    [Fact]
    public async Task Should_ReturnEmptyOrder_When_OrderHasNoPizzasAsync()
    {
        var order = Order.StartNewOrder(TestHelpers.OwnerUserId, Guid.NewGuid());
        order.DomainEvents.GetAndClear();

        var handler = CreateHandler(order);

        var result = await handler.Handle(new GetOrderQuery(order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Pizzas.Should().BeEmpty();
        result.Data.TotalCost.Should().Be(0);
    }

    [Fact]
    public async Task Should_FailNotFound_When_OrderDoesNotExistAsync()
    {
        var orderRepository = TestHelpers.Repository<Order>();
        orderRepository
            .Setup(x => x.QueryOneAsync(It.IsAny<QuerySpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var handler = new GetOrderQuery.Handler(orderRepository.Object);

        var result = await handler.Handle(new GetOrderQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    private static GetOrderQuery.Handler CreateHandler(Order order)
    {
        var orderRepository = TestHelpers.Repository<Order>();
        orderRepository
            .Setup(x => x.QueryOneAsync(It.IsAny<QuerySpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        return new GetOrderQuery.Handler(orderRepository.Object);
    }
}
