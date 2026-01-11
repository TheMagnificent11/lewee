using System.Diagnostics.CodeAnalysis;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using FluentAssertions;
using Lewee.Tests.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lewee.Infrastructure.Refit.Tests.Integration;

public sealed class HttpClientTests : IAsyncLifetime
{
    private IDistributedApplicationTestingBuilder builder;
    private DistributedApplication app;

    public async Task InitializeAsync()
    {
        // Create the test application
        this.builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Lewee_Tests_AppHost>();

        this.app = await this.builder.BuildAsync();

        // Get resource notification service before starting
        var resourceNotificationService = this.app.Services.GetRequiredService<ResourceNotificationService>();

        await this.app.StartAsync();

        // Wait for Web API to be running
        await resourceNotificationService
            .WaitForResourceAsync(ServiceNames.WebApi, KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromMinutes(5));
    }

    public async Task DisposeAsync()
    {
        if (this.app != null)
        {
            await this.app.StopAsync();
            await this.app.DisposeAsync();
        }

        if (this.builder != null)
        {
            await this.builder.DisposeAsync();
        }
    }

    [SuppressMessage("Usage", "xUnit1004:Test methods should not be skipped", Justification = "Temporary skip")]
    [Fact(Skip = "Temporary skip")]
    public async Task AddWebApiHttpClient_Should_CreateFunctioningHttpClient()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddWebApiHttpClient<IPizzaClient>(ServiceNames.WebApi);

        await using var serviceProvider = services.BuildServiceProvider();

        var httpClient = serviceProvider.GetRequiredService<IPizzaClient>();
        var request = new AddPizzaToMenuRequest("Margherita", 12.50m);

        // Act
        await httpClient.AddPizzaToMenuAsync(request, CancellationToken.None);
        var result = await httpClient.GetMenuAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().ContainSingle(x => x.Name == request.Name && x.Price == request.Price);
    }
}
