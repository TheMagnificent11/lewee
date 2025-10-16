using FluentAssertions;
using Pizzeria.Common;
using Xunit;

namespace Pizzeria.Tests.Integration;

[Collection(PizzeriaApplicationFactory.CollectionName)]
public sealed class HealthCheckTests : PizzeriaTests
{
    public HealthCheckTests(PizzeriaApplicationFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnHealthy_When_CallingLivenessEndpointAsync()
    {
        // Arrange
        using var httpClient = await this.factory.GetServiceClientAsync(ServiceNames.PizzaStoreApi);

        // Act
        using var response = await httpClient.GetAsync("/health");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue("the /health endpoint should return healthy for Aspire orchestration and include database connectivity check");
    }

    [Fact]
    public async Task Should_ReturnReadinessStatus_When_CallingReadinessEndpointAsync()
    {
        // Arrange
        using var httpClient = await this.factory.GetServiceClientAsync(ServiceNames.PizzaStoreApi);

        // Act
        using var response = await httpClient.GetAsync("/ready");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue("the /ready endpoint should return healthy after all startup services are configured");
    }

    [Fact]
    public async Task Should_ReturnAlive_When_CallingAliveEndpointInDevelopmentAsync()
    {
        // Arrange
        using var httpClient = await this.factory.GetServiceClientAsync(ServiceNames.PizzaStoreApi);

        // Act
        using var response = await httpClient.GetAsync("/alive");

        // Assert
        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue("the /alive endpoint should return healthy in development");
    }
}
