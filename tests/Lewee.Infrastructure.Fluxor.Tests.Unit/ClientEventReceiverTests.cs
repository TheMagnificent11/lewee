#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Bunit;
using FluentAssertions;
using Fluxor;
using Lewee.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lewee.Infrastructure.Fluxor.Tests.Unit;

[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test context handles disposal")]
[SuppressMessage("Reliability", "CA2213:Disposable fields should be disposed", Justification = "Test context handles disposal")]
public sealed class ClientEventReceiverTests : TestContext
{
    private readonly string testUserId = "test-user-id";
    private readonly Mock<IDispatcher> mockDispatcher = new();
    private readonly Mock<IMessageToActionMapper> mockMessageMapper = new();
    private readonly Mock<IAuthenticatedUserService> mockAuthService = new();
    private readonly TestSseClientMessageReceiver testMessageReceiver;
    private readonly Guid testCorrelationId = Guid.NewGuid();

    public ClientEventReceiverTests()
    {
        this.mockAuthService
            .Setup(x => x.UserId)
            .Returns(this.testUserId);

        var httpClient = new HttpClient();
        var logger = Mock.Of<ILogger<SseClientMessageReceiver>>();
        this.testMessageReceiver = new TestSseClientMessageReceiver(httpClient, logger);

        this.Services.AddFakeLogging();
        this.Services.AddSingleton<SseClientMessageReceiver>(this.testMessageReceiver);
        this.Services.AddSingleton(this.mockDispatcher.Object);
        this.Services.AddSingleton(this.mockMessageMapper.Object);
        this.Services.AddSingleton(this.mockAuthService.Object);
    }

    [Fact]
    public void OnInitialized_Should_StartMessageReceiver()
    {
        // Act
        this.RenderComponent<ClientEventReceiver>();

        // Assert
        this.testMessageReceiver.IsStarted.Should().BeTrue();
    }

    [Fact]
    public async Task DisposeAsync_Should_StopMessageReceiver()
    {
        // Arrange
        var cut = this.RenderComponent<ClientEventReceiver>();

        // Act
        await cut.Instance.DisposeAsync();

        // Assert
        this.testMessageReceiver.IsDisposed.Should().BeTrue();
    }

    [Fact]
    public void HandleClientMessage_Should_ProcessEvent_And_DispatchAction()
    {
        // Arrange
        var cut = this.RenderComponent<ClientEventReceiver>();
        var testMessage = new TestMessage { Value = "test" };
        var testAction = new TestAction();
        var clientMessage = this.CreateClientMessage(testMessage);

        this.mockMessageMapper
            .Setup(x => x.Map(It.IsAny<object>(), this.testCorrelationId))
            .Returns(testAction);

        // Act
        this.testMessageReceiver.SimulateMessageReceived(clientMessage);

        // Assert
        cut.WaitForAssertion(
            () => this.mockDispatcher.Verify(x => x.Dispatch(testAction), Times.Once),
            TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void HandleClientMessage_Should_NotDispatch_When_TypeCannotBeResolved()
    {
        // Arrange
        var cut = this.RenderComponent<ClientEventReceiver>();
        var clientMessage = new ClientMessage
        {
            CorrelationId = this.testCorrelationId,
            ContractAssemblyName = "NonExistent.Assembly",
            ContractFullClassName = "NonExistent.Type",
            MessageJson = "{}",
        };

        // Act
        this.testMessageReceiver.SimulateMessageReceived(clientMessage);

        // Assert
        cut.WaitForState(() => true, TimeSpan.FromMilliseconds(200));

        this.mockDispatcher.Verify(
            x => x.Dispatch(It.IsAny<object>()),
            Times.Never);
    }

    [Fact]
    public void HandleClientMessage_Should_NotDispatch_When_NoActionMapped()
    {
        // Arrange
        var cut = this.RenderComponent<ClientEventReceiver>();
        var testMessage = new TestMessage { Value = "test" };
        var clientMessage = this.CreateClientMessage(testMessage);

        this.mockMessageMapper
            .Setup(x => x.Map(It.IsAny<object>(), this.testCorrelationId))
            .Returns((IMessageReceivedAction?)null);

        // Act
        this.testMessageReceiver.SimulateMessageReceived(clientMessage);

        // Assert
        cut.WaitForState(() => true, TimeSpan.FromMilliseconds(200));

        this.mockDispatcher.Verify(
            x => x.Dispatch(It.IsAny<object>()),
            Times.Never);
    }

    private ClientMessage CreateClientMessage(TestMessage message)
    {
        var messageType = message.GetType();
        return new ClientMessage
        {
            CorrelationId = this.testCorrelationId,
            ContractAssemblyName = messageType.Assembly.FullName ?? string.Empty,
            ContractFullClassName = messageType.FullName ?? string.Empty,
            MessageJson = JsonSerializer.Serialize(message),
        };
    }

    private sealed class TestMessage
    {
        public string Value { get; set; } = string.Empty;
    }

    private sealed class TestAction : IMessageReceivedAction
    {
        public Guid CorrelationId { get; init; }
    }

    /// <summary>
    /// Test implementation of SseClientMessageReceiver that allows simulating message events
    /// </summary>
    [SuppressMessage(
        "Reliability",
        "CA2215:Dispose methods should call base class dispose",
        Justification = "Test double - no resources to dispose")]
    private sealed class TestSseClientMessageReceiver : SseClientMessageReceiver
    {
        public TestSseClientMessageReceiver(HttpClient httpClient, ILogger<SseClientMessageReceiver> logger)
            : base(httpClient, logger)
        {
        }

        public bool IsStarted { get; private set; }

        public bool IsDisposed { get; private set; }

        public override Task StartAsync(CancellationToken cancellationToken = default)
        {
            this.IsStarted = true;
            return Task.CompletedTask;
        }

        public override ValueTask DisposeAsync()
        {
            this.IsDisposed = true;
            return ValueTask.CompletedTask;
        }

        public void SimulateMessageReceived(ClientMessage message)
        {
            this.RaiseMessageReceived(message);
        }
    }
}
