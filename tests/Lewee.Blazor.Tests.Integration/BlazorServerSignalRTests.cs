using FluentAssertions;
using Lewee.Blazor.Tests.Integration.Infrastructure;
using Xunit;

namespace Lewee.Blazor.Tests.Integration;

/// <summary>
/// Tests for Blazor Server (in-process SignalR via channel) messaging scenario
/// </summary>
[Collection(BlazorServerTestFixture.CollectionName)]
public sealed class BlazorServerSignalRTests
{
    private readonly BlazorServerTestFixture fixture;

    public BlazorServerSignalRTests(BlazorServerTestFixture fixture)
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
    public async Task Should_CreateOrder_When_PostingToOrdersEndpointAsync()
    {
        // Act
        var order = await this.fixture.CreatePizzaOrderAsync();

        // Assert
        order.Should().NotBeNull();
        order!.Id.Should().NotBeEmpty();
        order.CustomerName.Should().Be("Test Customer");
    }
}
