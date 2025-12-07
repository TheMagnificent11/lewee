using System.Diagnostics.CodeAnalysis;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Lewee.Blazor.Tests.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Testing;
using Xunit;

namespace Lewee.Blazor.Tests.Integration;

[SuppressMessage(
    "Design",
    "CA1001:Types that own disposable fields should be disposable",
    Justification = "Disposed in `DisposeAsync`")]
[SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "xUnit requires this to be public")]
public sealed class TestFixture : IAsyncLifetime
{
    public const string CollectionName = "TestCollection";

    private IDistributedApplicationTestingBuilder builder;
    private DistributedApplication app;
    private HttpClient httpClient;
    private TestClient client;

    public async Task InitializeAsync()
    {
        this.builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Lewee_Blazor_Tests_App>();
        this.app = await this.builder.BuildAsync();
        var resourceNotificationService = this.app.Services.GetRequiredService<ResourceNotificationService>();

        await this.app.StartAsync();

        await resourceNotificationService
            .WaitForResourceAsync(ServiceNames.WebApi, KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromMinutes(5));

        this.httpClient = new HttpClient
        {
            BaseAddress = this.app.GetEndpoint(ServiceNames.WebApi),
        };

        this.client = new TestClient(this.httpClient);
    }

    public async Task DisposeAsync()
    {
        this.httpClient.Dispose();
        await this.app.DisposeAsync();
        await this.builder.DisposeAsync();
    }

    public IReadOnlyList<FakeLogRecord> GetClientLogs() => this.client.GetLogs();

    public async Task<bool> TestServerHealthAsync()
    {
        return await this.client.GetHealthAsync();
    }

    public async Task<bool> TestCreatePizzaOrderAsync()
    {
        return await this.client.CreatePizzaOrderAsync();
    }
}
