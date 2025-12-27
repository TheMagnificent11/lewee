using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Lewee.Tests.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lewee.Infrastructure.HttpClient.Tests.Integration;

public sealed class HttpClientTests : IAsyncLifetime
{
    private IDistributedApplicationTestingBuilder builder;
    private DistributedApplication app;
    private IServiceProvider serviceProvider;

    public async Task InitializeAsync()
    {
        // Create the test application
        this.builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Lewee_Tests_AppHost>();

        this.app = await this.builder.BuildAsync();

        // Get resource notification service before starting
        var resourceNotificationService = this.app.Services.GetRequiredService<ResourceNotificationService>();

        await this.app.StartAsync();

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddWebApiHttpClient<IPizzaClient>(ServiceNames.WebApi);

        this.serviceProvider = services.BuildServiceProvider();
    }

    public async Task DisposeAsync()
    {
        if (this.serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }

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
}
