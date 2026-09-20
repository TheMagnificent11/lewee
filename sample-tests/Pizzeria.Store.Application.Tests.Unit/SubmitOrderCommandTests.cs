using FluentAssertions;
using Lewee.Common;
using Lewee.Domain;
using Moq;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.Domain;
using Xunit;

namespace Pizzeria.Store.Application.Tests.Unit;

public sealed class SubmitOrderCommandTests
{
    [Fact]
    public async Task Should_SubmitPickupOrder_When_OrderOwnedAndNonEmptyAsync()
    {
        var order = TestHelpers.CreateOrderWithPizza(out _);
        var handler = CreatePickupHandler(order, TestHelpers.OwnerUserId);

        var result = await handler.Handle(new SubmitPickupOrderCommand(order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        order.IsSubmitted.Should().BeTrue();
        order.IsDeliveryOrder.Should().BeFalse();
        order.Status.Should().Be(OrderStatus.Received);
    }

    [Fact]
    public async Task Should_FailPickup_When_OrderOwnedByDifferentCallerAsync()
    {
        var order = TestHelpers.CreateOrderWithPizza(out _);
        var handler = CreatePickupHandler(order, TestHelpers.OtherUserId);

        var result = await handler.Handle(new SubmitPickupOrderCommand(order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Unauthorized);
        order.IsSubmitted.Should().BeFalse();
    }

    [Fact]
    public async Task Should_FailPickup_When_OrderHasNoPizzasAsync()
    {
        var order = Order.StartNewOrder(TestHelpers.OwnerUserId, Guid.NewGuid());
        order.DomainEvents.GetAndClear();
        var handler = CreatePickupHandler(order, TestHelpers.OwnerUserId);

        var result = await handler.Handle(new SubmitPickupOrderCommand(order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
    }

    [Fact]
    public async Task Should_SubmitDeliveryOrder_When_AddressSuppliedAsync()
    {
        var order = TestHelpers.CreateOrderWithPizza(out _);
        var handler = CreateDeliveryHandler(order, TestHelpers.OwnerUserId);

        var result = await handler.Handle(
            new SubmitDeliveryOrderCommand(order.Id, "1 Pizza Street"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        order.IsSubmitted.Should().BeTrue();
        order.IsDeliveryOrder.Should().BeTrue();
        order.DeliveryAddress.Should().Be("1 Pizza Street");
    }

    [Fact]
    public async Task Should_FailDelivery_When_OrderOwnedByDifferentCallerAsync()
    {
        var order = TestHelpers.CreateOrderWithPizza(out _);
        var handler = CreateDeliveryHandler(order, TestHelpers.OtherUserId);

        var result = await handler.Handle(
            new SubmitDeliveryOrderCommand(order.Id, "1 Pizza Street"),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Unauthorized);
    }

    [Fact]
    public async Task Should_FailSubmit_When_OrderAlreadySubmittedAsync()
    {
        var order = TestHelpers.CreateOrderWithPizza(out _);
        order.SubmitPickupOrder(Guid.NewGuid());
        order.DomainEvents.GetAndClear();

        var handler = CreatePickupHandler(order, TestHelpers.OwnerUserId);

        var result = await handler.Handle(new SubmitPickupOrderCommand(order.Id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
    }

    private static SubmitPickupOrderCommand.Handler CreatePickupHandler(Order order, string callerUserId)
    {
        return new SubmitPickupOrderCommand.Handler(
            OrderRepository(order).Object,
            TestHelpers.AuthenticatedUser(callerUserId).Object,
            TestHelpers.CorrelationAccessor().Object,
            TestHelpers.Logger<SubmitPickupOrderCommand.Handler>());
    }

    private static SubmitDeliveryOrderCommand.Handler CreateDeliveryHandler(Order order, string callerUserId)
    {
        return new SubmitDeliveryOrderCommand.Handler(
            OrderRepository(order).Object,
            TestHelpers.AuthenticatedUser(callerUserId).Object,
            TestHelpers.CorrelationAccessor().Object,
            TestHelpers.Logger<SubmitDeliveryOrderCommand.Handler>());
    }

    private static Mock<IRepository<Order>> OrderRepository(Order order)
    {
        var orderRepository = TestHelpers.Repository<Order>();
        orderRepository
            .Setup(x => x.QueryOneAsync(It.IsAny<QuerySpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        return orderRepository;
    }
}
