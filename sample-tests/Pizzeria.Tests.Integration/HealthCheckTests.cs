using FluentAssertions;
using Pizzeria.Common;
using Pizzeria.Tests.Integration.Infrastructure;
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
    public async Task Should_ServeTheWebAssemblyClient_When_LoadingGatewayClientRoot()
    {
        using var httpClient = await this.Factory.GetServiceClientAsync(ServiceNames.Gateway);
        using var response = await httpClient.GetAsync($"/{ServiceNames.PizzaStoreWeb}/");

        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("blazor.webassembly.js");
    }

    [Fact]
    public async Task Should_ExposeBlazorConfiguration_When_LoadingGatewayConfiguration()
    {
        using var httpClient = await this.Factory.GetServiceClientAsync(ServiceNames.Gateway);
        using var response = await httpClient.GetAsync(
            $"/{ServiceNames.PizzaStoreWeb}/_blazor/_configuration");

        response.Should().NotBeNull();
        response.IsSuccessStatusCode.Should().BeTrue();
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(ServiceNames.Bff);
    }
}
