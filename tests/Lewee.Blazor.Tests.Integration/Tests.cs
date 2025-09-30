using Xunit;

namespace Lewee.Blazor.Tests.Integration;

[Collection(TestServerFixture.CollectionName)]
public sealed class Tests 
{
    private readonly TestServerFixture testServer;

    public Tests(TestServerFixture testServer)
    {
        this.testServer = testServer;
    }

    [Fact]
    public async Task HealthCheck_ReturnsHealthy()
    {
        // Arrange
        using var client = this.testServer.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
