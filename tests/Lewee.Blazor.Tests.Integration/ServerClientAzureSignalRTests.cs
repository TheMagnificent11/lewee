using FluentAssertions;
using Lewee.Blazor.Tests.Integration.Infrastructure;
using Xunit;

namespace Lewee.Blazor.Tests.Integration;

[Collection(AzureSignalRTestFixture.CollectionName)]
public sealed class ServerClientAzureSignalRTests
{
    private readonly AzureSignalRTestFixture fixture;

    public ServerClientAzureSignalRTests(AzureSignalRTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task Should_ReceiveHttpOk_When_CallingHealthEndpointAsync()
    {
        // Act
        var result = await this.fixture.TestServerHealthAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Should_ReceivesSignalRMessage_When_PostIsSuccessfulAsync()
    {
        // Act
        var result = await this.fixture.TestCreatePizzaOrderAsync();

        // Assert
        result.Should().BeTrue();

        // Wait for SignalR message to be received
        await Task.Delay(TimeSpan.FromSeconds(1));

        var clientLogs = this.fixture.GetClientLogs();

        // Assert
        clientLogs.Should().NotBeNullOrEmpty();

        var actionToMapperLogs = clientLogs
            .Where(x => x.Category == typeof(MessageToActionMapper).FullName)
            .ToArray();

        actionToMapperLogs.Should().NotBeNullOrEmpty();
        actionToMapperLogs.Should().ContainSingle(x => x.Message.Contains("SignalR message received"));

        _ = actionToMapperLogs.FirstOrDefault();
    }
}
