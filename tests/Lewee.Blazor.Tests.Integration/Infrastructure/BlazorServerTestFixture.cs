using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Lewee.Tests.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Lewee.Blazor.Tests.Integration.Infrastructure;

/// <summary>
/// Test fixture for Blazor Server (in-process SignalR) messaging scenario
/// </summary>
[SuppressMessage(
    "Design",
    "CA1001:Types that own disposable fields should be disposable",
    Justification = "Disposed in `DisposeAsync`")]
[SuppressMessage(
    "Maintainability",
    "CA1515:Consider making public types internal",
    Justification = "xUnit requires this to be public")]
public sealed class BlazorServerTestFixture : IAsyncLifetime
{
    public const string CollectionName = "BlazorServerTestCollection";

    private IDistributedApplicationTestingBuilder builder = null!;
    private DistributedApplication app = null!;
    private HttpClient httpClient = null!;

    public async Task InitializeAsync()
    {
        this.builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Lewee_Blazor_Tests_App>();
        this.app = await this.builder.BuildAsync();
        var resourceNotificationService = this.app.Services.GetRequiredService<ResourceNotificationService>();

        await this.app.StartAsync();

        await resourceNotificationService
            .WaitForResourceAsync(ServiceNames.BlazorServerWeb, KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromMinutes(5));

        this.httpClient = this.app.CreateHttpClient(ServiceNames.BlazorServerWeb);

        await this.WaitForHealthAsync(TimeSpan.FromMinutes(1));
    }

    public async Task DisposeAsync()
    {
        this.httpClient.Dispose();
        await this.app.DisposeAsync();
        await this.builder.DisposeAsync();
    }

    public async Task<bool> TestServerHealthAsync()
    {
        try
        {
            using var response = await this.httpClient.GetAsync("/health");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<PizzaOrder> CreatePizzaOrderAsync()
    {
        var request = new CreateOrderRequest("Test Customer", "Margherita", 1);
        using var response = await this.httpClient.PostAsJsonAsync("/api/orders", request);

        response.EnsureSuccessStatusCode();

        var order = await response.Content.ReadFromJsonAsync<PizzaOrder>();

        return order!;
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
