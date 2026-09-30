using FluentAssertions;
using Lewee.Common;
using Lewee.Domain;
using MediatR;
using Moq;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.Domain;
using Xunit;

namespace Pizzeria.Store.Application.Tests.Unit;

public sealed class OrderOwnershipAuthorizationBehaviorTests
{
    [Fact]
    public async Task Should_InvokeHandler_When_OrderOwnedByCallerAsync()
    {
        var order = TestHelpers.CreateOrderWithPizza(out _);
        var behavior = CreateBehavior(order, TestHelpers.OwnerUserId);
        var nextCalled = false;

        RequestHandlerDelegate<QueryResult<OrderDto>> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(QueryResult<OrderDto>.Success(new OrderDto()));
        };

        var result = await behavior.Handle(new GetOrderQuery(order.Id), next, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Should_ReturnUnauthorized_When_OrderOwnedByDifferentCallerAsync()
    {
        var order = TestHelpers.CreateOrderWithPizza(out _);
        var behavior = CreateBehavior(order, TestHelpers.OtherUserId);
        var nextCalled = false;

        RequestHandlerDelegate<QueryResult<OrderDto>> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(QueryResult<OrderDto>.Success(new OrderDto()));
        };

        var result = await behavior.Handle(new GetOrderQuery(order.Id), next, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.Unauthorized);
        nextCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Should_ReturnNotFound_When_OrderDoesNotExistAsync()
    {
        var orderRepository = TestHelpers.Repository<Order>();
        orderRepository
            .Setup(x => x.QueryOneAsync(It.IsAny<QuerySpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var behavior = new OrderOwnershipAuthorizationBehavior<GetOrderQuery, QueryResult<OrderDto>>(
            orderRepository.Object,
            TestHelpers.AuthenticatedUser(TestHelpers.OwnerUserId).Object);

        var nextCalled = false;
        RequestHandlerDelegate<QueryResult<OrderDto>> next = (ct) =>
        {
            nextCalled = true;
            return Task.FromResult(QueryResult<OrderDto>.Success(new OrderDto()));
        };

        var result = await behavior.Handle(new GetOrderQuery(Guid.NewGuid()), next, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
        nextCalled.Should().BeFalse();
    }

    private static OrderOwnershipAuthorizationBehavior<GetOrderQuery, QueryResult<OrderDto>> CreateBehavior(
        Order order,
        string callerUserId)
    {
        var orderRepository = TestHelpers.Repository<Order>();
        orderRepository
            .Setup(x => x.QueryOneAsync(It.IsAny<QuerySpecification<Order>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        return new OrderOwnershipAuthorizationBehavior<GetOrderQuery, QueryResult<OrderDto>>(
            orderRepository.Object,
            TestHelpers.AuthenticatedUser(callerUserId).Object);
    }
}
