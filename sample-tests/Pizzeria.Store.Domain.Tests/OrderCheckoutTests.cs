using FluentAssertions;
using Lewee.Common;
using Pizzeria.Store.Contracts.Orders;
using Xunit;

namespace Pizzeria.Store.Domain.Tests;

public sealed class OrderCheckoutTests
{
    [Fact]
    public void SubmitPickupOrder_WithPizzas_Succeeds()
    {
        // Arrange
        var order = StartOrderWithPizza();

        // Act
        var result = order.SubmitPickupOrder(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.IsSubmitted.Should().BeTrue();
        order.DeliveryAddress.Should().BeNull();
        order.Status.Should().Be(OrderStatus.Received);
    }

    [Fact]
    public void SubmitDeliveryOrder_WithValidAddress_Succeeds()
    {
        // Arrange
        var order = StartOrderWithPizza();

        // Act
        var result = order.SubmitDeliveryOrder("1 Test Street", Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.IsSubmitted.Should().BeTrue();
        order.IsDeliveryOrder.Should().BeTrue();
        order.DeliveryAddress.Should().Be("1 Test Street");
        order.Status.Should().Be(OrderStatus.Received);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void SubmitDeliveryOrder_WithEmptyAddress_ReturnsBadRequest(string address)
    {
        // Arrange
        var order = StartOrderWithPizza();

        // Act
        var result = order.SubmitDeliveryOrder(address, Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        order.IsSubmitted.Should().BeFalse();
    }

    [Fact]
    public void SubmitPickupOrder_WithNoPizzas_ReturnsBadRequest()
    {
        // Arrange
        var order = Order.StartNewOrder(Guid.NewGuid().ToString(), Guid.NewGuid());

        // Act
        var result = order.SubmitPickupOrder(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        order.IsSubmitted.Should().BeFalse();
    }

    [Fact]
    public void SubmitDeliveryOrder_WithNoPizzas_ReturnsBadRequest()
    {
        // Arrange
        var order = Order.StartNewOrder(Guid.NewGuid().ToString(), Guid.NewGuid());

        // Act
        var result = order.SubmitDeliveryOrder("1 Test Street", Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        order.IsSubmitted.Should().BeFalse();
    }

    [Fact]
    public void SubmitPickupOrder_WhenAlreadySubmitted_ReturnsBadRequest()
    {
        // Arrange
        var order = StartOrderWithPizza();
        order.SubmitDeliveryOrder("1 Test Street", Guid.NewGuid());

        // Act
        var result = order.SubmitPickupOrder(Guid.NewGuid());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        order.IsDeliveryOrder.Should().BeTrue();
        order.DeliveryAddress.Should().Be("1 Test Street");
    }

    [Fact]
    public void SubmitPickupOrder_WhenAlreadySubmittedAsPickup_SucceedsWithoutChangingOrder()
    {
        var order = StartOrderWithPizza();
        order.SubmitPickupOrder(Guid.NewGuid());
        var submittedDateTime = order.SubmittedDateTime;
        var revision = order.Revision;
        order.DomainEvents.GetAndClear();

        var result = order.SubmitPickupOrder(Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
        order.SubmittedDateTime.Should().Be(submittedDateTime);
        order.Revision.Should().Be(revision);
        order.DomainEvents.GetAndClear().Should().BeEmpty();
    }

    [Fact]
    public void SubmitDeliveryOrder_WhenRepeatedWithSameAddress_SucceedsWithoutChangingOrder()
    {
        const string address = "1 Test Street";
        var order = StartOrderWithPizza();
        order.SubmitDeliveryOrder(address, Guid.NewGuid());
        var submittedDateTime = order.SubmittedDateTime;
        var revision = order.Revision;
        order.DomainEvents.GetAndClear();

        var result = order.SubmitDeliveryOrder(address, Guid.NewGuid());

        result.IsSuccess.Should().BeTrue();
        order.DeliveryAddress.Should().Be(address);
        order.SubmittedDateTime.Should().Be(submittedDateTime);
        order.Revision.Should().Be(revision);
        order.DomainEvents.GetAndClear().Should().BeEmpty();
    }

    private static Order StartOrderWithPizza()
    {
        var order = Order.StartNewOrder(Guid.NewGuid().ToString(), Guid.NewGuid());
        order.AddPizza(Menu.GetPizzaByName(Menu.PizzaNames.Margherita), Guid.NewGuid());
        return order;
    }
}
