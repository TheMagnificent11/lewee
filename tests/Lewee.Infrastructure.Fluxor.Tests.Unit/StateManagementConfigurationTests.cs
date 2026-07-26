using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lewee.Infrastructure.Fluxor.Tests.Unit;

public class StateManagementConfigurationTests
{
    [Fact]
    public void AddLeweeFluxor_ShouldRegisterFluxor()
    {
        var services = new ServiceCollection();

        var result = services.AddLeweeFluxor();

        result.Should().BeSameAs(services);
        services.Should().NotBeEmpty();
    }

    [Fact]
    public void AddLeweeFluxor_WithReduxDevTools_ShouldRegisterFluxorWithDevTools()
    {
        var services = new ServiceCollection();

        var result = services.AddLeweeFluxor(useReduxDevTools: true);

        result.Should().BeSameAs(services);
        services.Should().NotBeEmpty();
    }

    [Fact]
    public void AddLeweeFluxor_WithStateManagementAssemblies_ShouldRegisterFluxor()
    {
        var services = new ServiceCollection();
        var assembly = typeof(StateManagementConfiguration).Assembly;

        var result = services.AddLeweeFluxor(useReduxDevTools: false, assembly);

        result.Should().BeSameAs(services);
        services.Should().NotBeEmpty();
    }
}
