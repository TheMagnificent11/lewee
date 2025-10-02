using FreeMediator;
using Lewee.Application.Mediation.Notifications;
using Microsoft.Extensions.Logging;
using Moq;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Domain;
using Xunit;

namespace Pizzeria.Store.Application.Tests.Unit;

public class OrderingDomainEventHandlerTests
{
    [Fact]
    public async Task Handle_OrderStartedEvent_PublishesClientEvent()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var mockLogger = new Mock<ILogger<OrderingDomainEventHandler>>();
        var handler = new OrderingDomainEventHandler(mockMediator.Object, mockLogger.Object);

        var orderId = Guid.NewGuid();
        var userId = "test-user";
        var startedDateTime = DateTime.UtcNow;

        var orderStartedEvent = new OrderStartedEvent(orderId, userId, startedDateTime);

        // Act
        await handler.Handle(orderStartedEvent, CancellationToken.None);

        // Assert
        mockMediator.Verify(
            m => m.Publish(
                It.Is<ClientEvent>(ce =>
                    ce.CorrelationId == orderStartedEvent.CorrelationId &&
                    ce.UserId == userId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_OrderStartedEvent_CreatesCorrectDto()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var mockLogger = new Mock<ILogger<OrderingDomainEventHandler>>();
        var handler = new OrderingDomainEventHandler(mockMediator.Object, mockLogger.Object);

        var orderId = Guid.NewGuid();
        var userId = "test-user";
        var startedDateTime = DateTime.UtcNow;

        var orderStartedEvent = new OrderStartedEvent(orderId, userId, startedDateTime);

        ClientEvent capturedEvent = null!;
        mockMediator.Setup(m => m.Publish(It.IsAny<ClientEvent>(), It.IsAny<CancellationToken>()))
            .Callback<INotification, CancellationToken>((e, _) => capturedEvent = e as ClientEvent!);

        // Act
        await handler.Handle(orderStartedEvent, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedEvent);
        Assert.Contains(orderId.ToString(), capturedEvent.MessageJson, StringComparison.Ordinal);
        Assert.Contains(userId, capturedEvent.MessageJson, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Handle_OrderStartedEvent_LogsInformation()
    {
        // Arrange
        var mockMediator = new Mock<IMediator>();
        var mockLogger = new Mock<ILogger<OrderingDomainEventHandler>>();
        var handler = new OrderingDomainEventHandler(mockMediator.Object, mockLogger.Object);

        var orderId = Guid.NewGuid();
        var userId = "test-user";
        var startedDateTime = DateTime.UtcNow;

        var orderStartedEvent = new OrderStartedEvent(orderId, userId, startedDateTime);

        // Act
        await handler.Handle(orderStartedEvent, CancellationToken.None);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Handling OrderStartedEvent for order {orderId}", StringComparison.Ordinal)),
                It.IsAny<Exception>(),
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
#pragma warning restore CS8625
            Times.Once);
    }
}
