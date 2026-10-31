using Lewee.Application.Mediation.Notifications;
using Lewee.Domain;
using MediatR;
using Moq;
using Pizzeria.Store.Application.Orders;
using Pizzeria.Store.Domain;
using Xunit;

namespace Pizzeria.Store.Application.Tests.Unit;

public sealed class OrderingDomainEventHandlerTests
{
    [Theory]
    [InlineData("staff-user")]
    [InlineData(null)]
    public async Task Should_RouteOrderEventsToOwner_When_ActorIsDifferentOrMissing(string? actorUserId)
    {
        var order = Order.StartNewOrder(TestHelpers.OwnerUserId, Guid.NewGuid());
        var orderRepository = TestHelpers.Repository<Order>();
        orderRepository
            .Setup(item => item.QueryOneAsync(
                It.IsAny<QuerySpecification<Order>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        var queryProjectionService = new Mock<IQueryProjectionService>();
        queryProjectionService
            .Setup(item => item.AddOrUpdateAsync(
                It.IsAny<OrderQueryProjection>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var mediator = new Mock<IMediator>();
        mediator
            .Setup(item => item.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var handler = new OrderingDomainEventHandler(
            orderRepository.Object,
            queryProjectionService.Object,
            mediator.Object,
            TestHelpers.Logger<OrderingDomainEventHandler>());
        var correlationId = Guid.NewGuid();

        await handler.Handle(
            new OrderCompletedEvent(order.Id, actorUserId!, DateTime.UtcNow, correlationId),
            CancellationToken.None);

        mediator.Verify(
            item => item.Publish(
                It.Is<ClientEvent>(clientEvent =>
                    clientEvent.UserId == order.UserId &&
                    clientEvent.CorrelationId == correlationId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
