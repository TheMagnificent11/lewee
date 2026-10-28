using FluentAssertions;
using Lewee.Infrastructure.Fluxor;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Pizzeria.Ordering.StateManagement.Orders;
using Pizzeria.Ordering.StateManagement.Orders.Actions;
using Pizzeria.Ordering.Web.Infrastructure;
using Pizzeria.Store.Contracts.Orders;
using Xunit;

namespace Pizzeria.Ordering.Web.Tests.Unit;

public class MessageToActionMapperTests
{
    private readonly Mock<IOrderCommandCorrelationTracker> correlationTrackerMock = new();
    private readonly MessageToActionMapper mapper;

    public MessageToActionMapperTests()
    {
        this.mapper = new MessageToActionMapper(
            this.correlationTrackerMock.Object,
            NullLogger<MessageToActionMapper>.Instance);
    }

    [Fact]
    public void Map_WithUnrecognisedMessageType_ReturnsNull()
    {
        // Act
        var result = this.mapper.Map(new object(), Guid.NewGuid());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Map_WithOrderDtoAndNoPendingCommand_ReturnsNull()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var order = new OrderDto();
        var outCommandType = default(OrderCommandType);

        this.correlationTrackerMock
            .Setup(t => t.TryGetAndRemove(correlationId, out outCommandType))
            .Returns(false);

        // Act
        var result = this.mapper.Map(order, correlationId);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(OrderCommandType.StartOrder, typeof(StartOrderCompletedAction))]
    [InlineData(OrderCommandType.AddPizzaToOrder, typeof(AddPizzaToOrderCompletedAction))]
    [InlineData(OrderCommandType.RemovePizzaFromOrder, typeof(RemovePizzaFromOrderCompletedAction))]
    [InlineData(OrderCommandType.SubmitPickupOrder, typeof(SubmitPickupOrderCompletedAction))]
    [InlineData(OrderCommandType.SubmitDeliveryOrder, typeof(SubmitDeliveryOrderCompletedAction))]
    public void Map_WithOrderDtoAndPendingCommand_ReturnsExpectedAction(
        OrderCommandType commandType,
        Type expectedActionType)
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var order = new OrderDto { Id = Guid.NewGuid() };

        this.correlationTrackerMock
            .Setup(t => t.TryGetAndRemove(correlationId, out commandType))
            .Returns(true);

        // Act
        var result = this.mapper.Map(order, correlationId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType(expectedActionType);
        result.CorrelationId.Should().Be(correlationId);
        ((IMessageReceivedAction<OrderDto>)result).Data.Should().Be(order);
    }
}
