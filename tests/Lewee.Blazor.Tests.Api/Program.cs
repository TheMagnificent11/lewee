using Lewee.Application.Mediation.Notifications;
using Lewee.Blazor.Tests.Contracts;
using Lewee.Infrastructure.AspNet.SignalR;
using Lewee.Tests.ServiceDefaults;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services
    .AddRouting()
    .AddLeweeSignalR(builder.Configuration.GetConnectionString(ServiceNames.SignalR));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.MapDefaultEndpoints();
app.MapLeweeSignalRNegotiateEndpoint();

app.MapPost("/api/orders", async (CreateOrderRequest request, IMediator mediator) =>
{
    var order = new PizzaOrder(
        Id: Guid.NewGuid(),
        CustomerName: "Test User",
        CreatedAt: DateTime.UtcNow);

    await mediator.Publish(new ClientEvent(Guid.NewGuid(), userId: null, order));

    return Results.Ok();
});

await app.RunAsync();
