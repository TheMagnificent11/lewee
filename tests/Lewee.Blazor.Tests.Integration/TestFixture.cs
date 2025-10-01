using System.Collections.Concurrent;
using FreeMediator;
using Lewee.Application.Mediation.Notifications;
using Lewee.Infrastructure.AspNet.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Xunit;

namespace Lewee.Blazor.Tests.Integration;

public sealed class TestFixture : IAsyncLifetime
{
    public const string CollectionName = "TestCollection";

    public static readonly ConcurrentDictionary<Guid, PizzaOrder> Orders = new();
    private readonly TestServer server;
    private readonly TestClient client;
    private readonly HttpClient httpClient;
    private readonly FakeLogCollector serverLogCollector;

    public TestFixture()
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services
                    .AddFakeLogging()
                    .AddRouting()
                    .AddLeweeSignalR();

                services.AddMediator(options => { });
                services.AddHealthChecks();
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapHealthChecks("/health");

                    endpoints.MapHub<ClientEventHub>("/events");

                    endpoints.MapPost("/api/orders", async (CreateOrderRequest request, IMediator mediator) =>
                    {
                        var order = new PizzaOrder
                        {
                            Id = Guid.NewGuid(),
                            CustomerName = "Test User",
                            CreatedAt = DateTime.UtcNow
                        };
                        
                        Orders.TryAdd(order.Id, order);
                        
                        await mediator.Publish(new ClientEvent(Guid.NewGuid(), userId: null, order));

                        return Results.Ok();
                    });
                    
                    endpoints.MapGet("/api/orders/{id}", (Guid id) =>
                    {
                        return Orders.TryGetValue(id, out var order) 
                            ? Results.Ok(order) 
                            : Results.NotFound();
                    });
                });
            });

        this.server = new TestServer(builder);
        this.httpClient = this.server.CreateClient();
        this.client = new TestClient(this.httpClient);
        this.serverLogCollector = this.server.Services.GetRequiredService<FakeLogCollector>();
    }

    public async Task InitializeAsync()
    {
        await this.client.ConnectAsync();
    }

    public async Task DisposeAsync()
    {
        await this.client.DisconnectAsync();
        this.client.Dispose();
        this.httpClient.Dispose();
        this.server.Dispose();
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
