using Pizzeria.Store.Contracts;
using Pizzeria.Store.WebClient.States;
using Pizzeria.Store.WebClient.States.Orders.Actions;

namespace Pizzeria.Store.WebClient.Tests.Unit;

public class MessageToActionMapperTests
{
    [Fact]
    public void Map_OrderStartedEventDto_ReturnsStartOrderCompletedAction()
    {
        // Arrange
        var mapper = new MessageToActionMapper();
        var orderId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var message = new OrderStartedEventDto
        {
            OrderId = orderId,
            UserId = "test-user",
            StartedDateTime = DateTime.UtcNow
        };

        // Act
        var action = mapper.Map(message, correlationId);

        // Assert
        Assert.NotNull(action);
        Assert.IsType<StartOrderCompletedAction>(action);
        var completedAction = (StartOrderCompletedAction)action;
        Assert.Equal(orderId, completedAction.OrderId);
        Assert.Equal(correlationId, completedAction.CorrelationId);
    }

    [Fact]
    public void Map_UnknownMessageType_ReturnsNull()
    {
        // Arrange
        var mapper = new MessageToActionMapper();
        var message = new { SomeProperty = "value" };
        var correlationId = Guid.NewGuid();

        // Act
        var action = mapper.Map(message, correlationId);

        // Assert
        Assert.Null(action);
    }

    [Fact]
    public void Map_NullMessage_ReturnsNull()
    {
        // Arrange
        var mapper = new MessageToActionMapper();
        var correlationId = Guid.NewGuid();

        // Act
        var action = mapper.Map(null!, correlationId);

        // Assert
        Assert.Null(action);
    }
}
