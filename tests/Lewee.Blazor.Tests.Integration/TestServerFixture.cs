using System.Collections.Concurrent;
using Lewee.Infrastructure.AspNet.SignalR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Lewee.Blazor.Tests.Integration;

public sealed class TestServerFixture : IDisposable
{
    public const string CollectionName = "TestServerCollection";

    private readonly TestServer server;

    private bool disposedValue;

    public static readonly ConcurrentDictionary<Guid, PizzaOrder> Orders = new();

    public TestServerFixture()
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services
                    .AddFakeLogging()
                    .AddRouting()
                    .AddSignalR();
                
                services
                    .AddLeweeBlazor<MessageToActionMapper>(new Uri("http://localhost"), useReduxDevTools: false)
                    .AddHealthChecks();
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapHealthChecks("/health");
                    endpoints.MapHub<ClientEventHub>("/events");
                    
                    endpoints.MapPost("/api/orders", async (CreateOrderRequest request, IHubContext<ClientEventHub> hubContext) =>
                    {
                        var order = new PizzaOrder
                        {
                            Id = Guid.NewGuid(),
                            CustomerName = request.CustomerName,
                            PizzaType = request.PizzaType,
                            Quantity = request.Quantity,
                            CreatedAt = DateTime.UtcNow
                        };
                        
                        Orders.TryAdd(order.Id, order);
                        
                        // Send SignalR message to all connected clients
                        await hubContext.Clients.All.SendAsync("OrderCreated", order);
                        
                        return Results.Ok(order);
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
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public HttpClient CreateClient() => this.server.CreateClient();

    private void Dispose(bool disposing)
    {
        if (this.disposedValue)
        {
            return;
        }

        if (disposing)
        {
            this.server.Dispose();
        }

        this.disposedValue = true;
    }
}

public record CreateOrderRequest(string CustomerName, string PizzaType, int Quantity);

public record PizzaOrder
{
    public Guid Id { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string PizzaType { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public DateTime CreatedAt { get; init; }
}
