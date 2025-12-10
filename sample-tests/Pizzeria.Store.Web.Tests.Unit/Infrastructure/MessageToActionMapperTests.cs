using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Pizzeria.Store.Contracts.Orders;
using Pizzeria.Store.StateManagement.Orders.Actions;
using Pizzeria.Store.Web.Infrastructure;
using Xunit;

namespace Pizzeria.Store.Web.Tests.Unit.Infrastructure;

public class MessageToActionMapperTests
{
    [Fact]
    public void Map_OrderDto_ReturnsStartOrderCompletedAction()
    {
        // Arrange
        var mapper = new MessageToActionMapper(Mock.Of<ILogger<MessageToActionMapper>>());
        var correlationId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var message = new OrderDto
        {
            Id = orderId,
            UserId = "test-user",
            StartedDateTime = DateTime.UtcNow,
            Pizzas = [],
            TotalCost = 0,
        };

        // Act
        var action = mapper.Map(message, correlationId);

        // Assert
        action.Should().NotBeNull();
        action.Should().BeOfType<StartOrderCompletedAction>();
        var completedAction = (StartOrderCompletedAction)action!;
        completedAction.Order.Id.Should().Be(orderId);
        completedAction.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public void Map_UnknownMessageType_ReturnsNull()
    {
        // Arrange
        var logger = new Mock<ILogger<MessageToActionMapper>>();
        var mapper = new MessageToActionMapper(logger.Object);
        var message = new { SomeProperty = "value" };
        var correlationId = Guid.NewGuid();

        // Act
        var action = mapper.Map(message, correlationId);

        // Assert
        action.Should().BeNull();
    }
}
