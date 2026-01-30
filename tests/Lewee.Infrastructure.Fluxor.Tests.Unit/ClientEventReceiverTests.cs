using Bunit;
using Fluxor;
using Lewee.Application.Mediation.Notifications;
using Lewee.Application.ServerSentEvents;
using Lewee.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lewee.Infrastructure.Fluxor.Tests.Unit;

public sealed class ClientEventReceiverTests : TestContext
{
    private readonly string testUserId = "test-user-id";
    private readonly Mock<IClientEventBroadcaster> mockBroadcaster = new();
    private readonly Mock<IDispatcher> mockDispatcher = new();
    private readonly Mock<IMessageToActionMapper> mockMessageMapper = new();
    private readonly Mock<IAuthenticatedUserService> mockAuthService = new();
    private readonly Guid testCorrelationId = Guid.NewGuid();

    public ClientEventReceiverTests()
    {
        this.mockAuthService
            .Setup(x => x.UserId)
            .Returns(this.testUserId);

        this.Services.AddFakeLogging();
        this.Services.AddSingleton(this.mockBroadcaster.Object);
        this.Services.AddSingleton(this.mockDispatcher.Object);
        this.Services.AddSingleton(this.mockMessageMapper.Object);
        this.Services.AddSingleton(this.mockAuthService.Object);
    }

    [Fact]
    public void OnInitialized_Should_SubscribeToClientEvents()
    {
        // Act
        this.RenderComponent<ClientEventReceiver>();

        // Assert
        this.mockBroadcaster.VerifyAdd(
            x => x.OnClientEvent += It.IsAny<EventHandler<ClientEventArgs>>(),
            Times.Once);
    }

    [Fact]
    public void Dispose_Should_UnsubscribeFromClientEvents()
    {
        // Arrange
        var cut = this.RenderComponent<ClientEventReceiver>();

        // Act
        cut.Instance.Dispose();

        // Assert
        this.mockBroadcaster.VerifyRemove(
            x => x.OnClientEvent -= It.IsAny<EventHandler<ClientEventArgs>>(),
            Times.Once);
    }

    [Fact]
    public void HandleClientEvent_Should_ProcessEvent_When_UserIdMatches()
    {
        // Arrange
        var cut = this.RenderComponent<ClientEventReceiver>();
        var testMessage = new TestMessage { Value = "test" };
        var testAction = new TestAction();
        var clientEvent = this.CreateClientEvent(this.testUserId, testMessage);

        this.mockMessageMapper
            .Setup(x => x.Map(It.IsAny<object>(), this.testCorrelationId))
            .Returns(testAction);

        // Act
        this.mockBroadcaster.Raise(
            x => x.OnClientEvent += null,
            new ClientEventArgs(clientEvent));

        // Assert
        cut.WaitForAssertion(
            () => this.mockDispatcher.Verify(x => x.Dispatch(testAction), Times.Once),
            TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void HandleClientEvent_Should_NotProcessEvent_When_UserIdDoesNotMatch()
    {
        // Arrange
        var cut = this.RenderComponent<ClientEventReceiver>();
        var testMessage = new TestMessage { Value = "test" };
        var clientEvent = this.CreateClientEvent("different-user-id", testMessage);

        // Act
        this.mockBroadcaster.Raise(
            x => x.OnClientEvent += null,
            new ClientEventArgs(clientEvent));

        // Assert
        cut.WaitForState(() => true, TimeSpan.FromMilliseconds(200));

        this.mockMessageMapper.Verify(
            x => x.Map(It.IsAny<object>(), It.IsAny<Guid>()),
            Times.Never);
        this.mockDispatcher.Verify(
            x => x.Dispatch(It.IsAny<object>()),
            Times.Never);
    }

    [Fact]
    public void HandleClientEvent_Should_ProcessEvent_When_UserIdIsNull()
    {
        // Arrange
        var cut = this.RenderComponent<ClientEventReceiver>();
        var testMessage = new TestMessage { Value = "test" };
        var testAction = new TestAction();
        var clientEvent = this.CreateClientEvent(userId: null, testMessage);

        this.mockMessageMapper
            .Setup(x => x.Map(It.IsAny<object>(), this.testCorrelationId))
            .Returns(testAction);

        // Act
        this.mockBroadcaster.Raise(
            x => x.OnClientEvent += null,
            new ClientEventArgs(clientEvent));

        // Assert
        cut.WaitForAssertion(
            () => this.mockDispatcher.Verify(x => x.Dispatch(testAction), Times.Once),
            TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void HandleClientEvent_Should_NotDispatch_When_TypeCannotBeResolved()
    {
        // Arrange
        var cut = this.RenderComponent<ClientEventReceiver>();
        var clientEvent = new ClientEvent(
            this.testCorrelationId,
            this.testUserId,
            new { Test = "1" });

        // Act
        this.mockBroadcaster.Raise(
            x => x.OnClientEvent += null,
            new ClientEventArgs(clientEvent));

        // Assert
        cut.WaitForState(() => true, TimeSpan.FromMilliseconds(200));

        this.mockDispatcher.Verify(
            x => x.Dispatch(It.IsAny<object>()),
            Times.Never);
    }

    [Fact]
    public void HandleClientEvent_Should_NotDispatch_When_NoActionMapped()
    {
        // Arrange
        var cut = this.RenderComponent<ClientEventReceiver>();
        var testMessage = new TestMessage { Value = "test" };
        var clientEvent = this.CreateClientEvent(this.testUserId, testMessage);

        this.mockMessageMapper
            .Setup(x => x.Map(It.IsAny<object>(), this.testCorrelationId))
            .Returns((IMessageReceivedAction)null!);

        // Act
        this.mockBroadcaster.Raise(
            x => x.OnClientEvent += null,
            new ClientEventArgs(clientEvent));

        // Assert
        cut.WaitForState(() => true, TimeSpan.FromMilliseconds(200));

        this.mockDispatcher.Verify(
            x => x.Dispatch(It.IsAny<object>()),
            Times.Never);
    }

    private ClientEvent CreateClientEvent(string userId, TestMessage message)
    {
        return new ClientEvent(this.testCorrelationId, userId, message);
    }

    private sealed class TestMessage
    {
        public string Value { get; set; } = string.Empty;
    }

    private sealed class TestAction : IMessageReceivedAction
    {
        public Guid CorrelationId { get; init; }
    }
}
