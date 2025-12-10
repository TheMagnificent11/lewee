using Lewee.Application.Mediation.Notifications;
using Lewee.Blazor.Messaging;
using Lewee.Blazor.Tests.Contracts;
using Lewee.Blazor.Tests.Web;
using Lewee.Blazor.Tests.Web.Components;
using Lewee.Infrastructure.AspNet.SignalR;
using Lewee.Tests.ServiceDefaults;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Server services
builder.Services
    .AddRouting()
    .AddLeweeSignalR()
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// Client services (for Blazor Server message receiving)
builder.Services
    .AddSignalRMessageReceiver<MessageToActionMapper>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAntiforgery();

app.MapDefaultEndpoints();
app.MapLeweeSignalRHub();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Test endpoint that publishes a client event
app.MapPost("/api/orders", async (CreateOrderRequest request, IMediator mediator) =>
{
    var order = new PizzaOrder(
        Id: Guid.NewGuid(),
        CustomerName: request.CustomerName,
        CreatedAt: DateTime.UtcNow);

    await mediator.Publish(new ClientEvent(Guid.NewGuid(), userId: null, order));

    return Results.Ok(order);
});

await app.RunAsync();
