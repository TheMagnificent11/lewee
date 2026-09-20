using FluentAssertions;
using Lewee.Common;
using Pizzeria.Store.Contracts.Orders;
using Xunit;

namespace Pizzeria.Store.Domain.Tests;

public sealed class OrderStatusTests
{
    [Fact]
    public void NewOrder_IsInProgress()
    {
        var order = Order.StartNewOrder(Guid.NewGuid().ToString(), Guid.NewGuid());

        order.Status.Should().Be(OrderStatus.InProgress);
    }

    [Fact]
    public void SubmittedOrder_IsReceived()
    {
        SubmittedPickupOrder().Status.Should().Be(OrderStatus.Received);
    }

    [Fact]
    public void StartMaking_FromReceived_TransitionsToMaking()
    {
        var order = SubmittedPickupOrder();

        var result = order.StartMaking(Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Making);
    }

    [Fact]
    public void StartMaking_WhenNotReceived_ReturnsBadRequest()
    {
        var order = Order.StartNewOrder(Guid.NewGuid().ToString(), Guid.NewGuid());

        var result = order.StartMaking(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        order.Status.Should().Be(OrderStatus.InProgress);
    }

    [Fact]
    public void Prepared_PickupOrder_TransitionsToReadyForPickup()
    {
        var order = SubmittedPickupOrder();
        order.StartMaking(Guid.NewGuid());

        var result = order.Prepared(Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.ReadyForPickup);
    }

    [Fact]
    public void Prepared_DeliveryOrder_TransitionsToReadyForDelivery()
    {
        var order = SubmittedDeliveryOrder();
        order.StartMaking(Guid.NewGuid());

        var result = order.Prepared(Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.ReadyForDelivery);
    }

    [Fact]
    public void Prepared_WhenNotSubmitted_ReturnsBadRequest()
    {
        var order = Order.StartNewOrder(Guid.NewGuid().ToString(), Guid.NewGuid());

        var result = order.Prepared(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
    }

    [Fact]
    public void OutForDelivery_FromReadyForDelivery_TransitionsToDelivering()
    {
        var order = SubmittedDeliveryOrder();
        order.Prepared(Guid.NewGuid());

        var result = order.OutForDelivery(Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Delivering);
    }

    [Fact]
    public void OutForDelivery_WhenNotReadyForDelivery_ReturnsBadRequest()
    {
        var order = SubmittedPickupOrder();
        order.Prepared(Guid.NewGuid());

        var result = order.OutForDelivery(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        order.Status.Should().Be(OrderStatus.ReadyForPickup);
    }

    [Fact]
    public void PickedUp_WhenPrepared_TransitionsToCompleted()
    {
        var order = SubmittedPickupOrder();
        order.Prepared(Guid.NewGuid());

        var result = order.PickedUp(Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Completed);
        order.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public void PickedUp_WhenNotPrepared_ReturnsBadRequest()
    {
        var order = SubmittedPickupOrder();

        var result = order.PickedUp(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        order.IsCompleted.Should().BeFalse();
    }

    [Fact]
    public void Delivered_WhenPrepared_TransitionsToCompleted()
    {
        var order = SubmittedDeliveryOrder();
        order.Prepared(Guid.NewGuid());

        var result = order.Delivered(Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Completed);
        order.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public void Delivered_AfterOutForDelivery_TransitionsToCompleted()
    {
        var order = SubmittedDeliveryOrder();
        order.Prepared(Guid.NewGuid());
        order.OutForDelivery(Guid.NewGuid());

        var result = order.Delivered(Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Completed);
    }

    [Fact]
    public void Delivered_WhenNotPrepared_ReturnsBadRequest()
    {
        var order = SubmittedDeliveryOrder();

        var result = order.Delivered(Guid.NewGuid());

        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        order.IsCompleted.Should().BeFalse();
    }

    private static Order SubmittedPickupOrder()
    {
        var order = Order.StartNewOrder(Guid.NewGuid().ToString(), Guid.NewGuid());
        order.AddPizza(Menu.GetPizzaByName(Menu.PizzaNames.Margherita));
        order.SubmitPickupOrder(Guid.NewGuid());
        return order;
    }

    private static Order SubmittedDeliveryOrder()
    {
        var order = Order.StartNewOrder(Guid.NewGuid().ToString(), Guid.NewGuid());
        order.AddPizza(Menu.GetPizzaByName(Menu.PizzaNames.Margherita));
        order.SubmitDeliveryOrder("1 Test Street", Guid.NewGuid());
        return order;
    }
}
