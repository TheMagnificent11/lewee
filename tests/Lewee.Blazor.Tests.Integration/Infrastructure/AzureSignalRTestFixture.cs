using System.Diagnostics.CodeAnalysis;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Lewee.Tests.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Testing;
using Xunit;

namespace Lewee.Blazor.Tests.Integration.Infrastructure;

/// <summary>
/// Test fixture for Azure SignalR (server-client) messaging scenario
/// </summary>
[SuppressMessage(
    "Design",
    "CA1001:Types that own disposable fields should be disposable",
    Justification = "Disposed in `DisposeAsync`")]
[SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "xUnit requires this to be public")]
public sealed class AzureSignalRTestFixture : IAsyncLifetime
{
    public const string CollectionName = "AzureSignalRTestCollection";

    private IDistributedApplicationTestingBuilder builder = null!;
    private DistributedApplication app = null!;
    private HttpClient httpClient = null!;
    private TestClient client = null!;

    public async Task InitializeAsync()
    {
        this.builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Lewee_Blazor_Tests_AppHost>();
        this.app = await this.builder.BuildAsync();
        var resourceNotificationService = this.app.Services.GetRequiredService<ResourceNotificationService>();

        await this.app.StartAsync();

        await resourceNotificationService
            .WaitForResourceAsync(ServiceNames.WebApi, KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromMinutes(1));

        this.httpClient = this.app.CreateHttpClient(ServiceNames.WebApi);

        await this.WaitForHealthAsync(TimeSpan.FromMinutes(1));

        this.client = new TestClient(this.httpClient);

        // Connect the SignalR hub
        await this.client.ConnectAsync();
    }

    public async Task DisposeAsync()
    {
        await this.client.DisconnectAsync();
        this.client.Dispose();
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

    private async Task WaitForHealthAsync(TimeSpan timeout)
    {
        var endTime = DateTime.UtcNow.Add(timeout);

        while (DateTime.UtcNow < endTime)
        {
            try
            {
                using var response = await this.httpClient.GetAsync("/health");
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch
            {
                // Ignore exceptions and continue polling
            }

            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        throw new TimeoutException($"Health check timed out after {timeout.TotalMinutes} minutes. The /health endpoint did not return a successful response.");
    }
}
