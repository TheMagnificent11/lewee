using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Lewee.Application.Mediation.Notifications;
using Lewee.Infrastructure.AspNet.SignalR;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

    [SuppressMessage(
        "Minor Bug",
        "S3887:Mutable, non-private fields should not be \"readonly\"",
        Justification = "Only for test purposes")]
    public static readonly ConcurrentDictionary<Guid, PizzaOrder> Orders = new();

    private TestServer server = null!;
    private TestClient client = null!;
    private HttpClient httpClient = null!;
    private FakeLogCollector serverLogCollector = null!;
    private WebApplication app = null!;

    public async Task InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Services
            .AddFakeLogging()
            .AddRouting()
            .AddLeweeSignalR();

        builder.Services.AddMediatR(options => options.RegisterServicesFromAssembly(typeof(ClientEvent).Assembly));
        builder.Services.AddHealthChecks();

        this.app = builder.Build();
        this.app.UseRouting();

        this.app.MapHealthChecks("/health");
        this.app.MapHub<ClientEventHub>("/events");

        this.app.MapPost("/api/orders", async (CreateOrderRequest request, IMediator mediator) =>
        {
            var order = new PizzaOrder
            {
                Id = Guid.NewGuid(),
                CustomerName = "Test User",
                CreatedAt = DateTime.UtcNow,
            };

            Orders.TryAdd(order.Id, order);

            await mediator.Publish(new ClientEvent(Guid.NewGuid(), userId: null, order));

            return Results.Ok();
        });

        this.app.MapGet("/api/orders/{id}", (Guid id) =>
        {
            return Orders.TryGetValue(id, out var order)
                ? Results.Ok(order)
                : Results.NotFound();
        });

        await this.app.StartAsync();
        this.server = this.app.GetTestServer();
        this.httpClient = this.server.CreateClient();
        this.client = new TestClient(this.httpClient);
        this.serverLogCollector = this.app.Services.GetRequiredService<FakeLogCollector>();

        await this.client.ConnectAsync();
    }

    public async Task DisposeAsync()
    {
        await this.client.DisconnectAsync();
        this.client.Dispose();
        this.httpClient.Dispose();
        this.server.Dispose();
        await this.app.StopAsync();
        await this.app.DisposeAsync();
    }

    public IReadOnlyList<FakeLogRecord> GetServerLogs() => this.serverLogCollector.GetSnapshot();

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
