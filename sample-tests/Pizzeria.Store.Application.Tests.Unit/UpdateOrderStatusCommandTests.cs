using FluentAssertions;
using Lewee.Common;
using Lewee.Domain;
using Moq;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.Domain;
using Xunit;

namespace Pizzeria.Store.Application.Tests.Unit;

public sealed class UpdateOrderStatusCommandTests
{
    [Theory]
    [InlineData(OrderFulfillmentAction.StartMaking, OrderStatus.Making)]
    [InlineData(OrderFulfillmentAction.Prepared, OrderStatus.ReadyForPickup)]
    [InlineData(OrderFulfillmentAction.PickedUp, OrderStatus.Completed)]
    [InlineData(OrderFulfillmentAction.OutForDelivery, OrderStatus.Delivering)]
    [InlineData(OrderFulfillmentAction.Delivered, OrderStatus.Completed)]
    public async Task Should_ApplyFulfillmentAction_When_OrderExistsAsync(
        OrderFulfillmentAction action,
        OrderStatus expectedStatus)
    {
        var order = CreateOrderForAction(action);
        var repository = TestHelpers.Repository<Order>();
        repository
            .Setup(item => item.QueryOneAsync(
                It.IsAny<QuerySpecification<Order>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        var handler = new UpdateOrderStatusCommand.Handler(
            repository.Object,
            TestHelpers.CorrelationAccessor().Object,
            TestHelpers.Logger<UpdateOrderStatusCommand.Handler>());

        var result = await handler.Handle(
            new UpdateOrderStatusCommand(order.Id, action),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(expectedStatus);
        repository.Verify(item => item.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_ReturnNotFound_When_OrderDoesNotExistAsync()
    {
        var repository = TestHelpers.Repository<Order>();
        repository
            .Setup(item => item.QueryOneAsync(
                It.IsAny<QuerySpecification<Order>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);
        var handler = new UpdateOrderStatusCommand.Handler(
            repository.Object,
            TestHelpers.CorrelationAccessor().Object,
            TestHelpers.Logger<UpdateOrderStatusCommand.Handler>());

        var result = await handler.Handle(
            new UpdateOrderStatusCommand(Guid.NewGuid(), OrderFulfillmentAction.StartMaking),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.NotFound);
    }

    private static Order CreateOrderForAction(OrderFulfillmentAction action)
    {
        var order = TestHelpers.CreateOrderWithPizza(out _);
        var isDelivery = action is OrderFulfillmentAction.OutForDelivery or OrderFulfillmentAction.Delivered;
        if (isDelivery)
        {
            order.SubmitDeliveryOrder("1 Test Street", Guid.NewGuid());
        }
        else
        {
            order.SubmitPickupOrder(Guid.NewGuid());
        }

        if (action is OrderFulfillmentAction.Prepared or OrderFulfillmentAction.PickedUp ||
            action is OrderFulfillmentAction.OutForDelivery or OrderFulfillmentAction.Delivered)
        {
            order.Prepared(Guid.NewGuid());
        }

        if (action == OrderFulfillmentAction.Delivered)
        {
            order.OutForDelivery(Guid.NewGuid());
        }

        order.DomainEvents.GetAndClear();
        return order;
    }
}
