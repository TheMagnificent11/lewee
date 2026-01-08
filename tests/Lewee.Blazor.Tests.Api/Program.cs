using Lewee.Application.Mediation.Notifications;
using Lewee.Infrastructure.AspNet.SignalR;
using Lewee.Tests.Contracts;
using Lewee.Tests.ServiceDefaults;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services
    .AddRouting()
    .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ClientEvent).Assembly))
    .AddLeweeAzureSignalR(builder.Configuration.GetConnectionString(ServiceNames.SignalR)!);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.MapDefaultEndpoints();
app.MapLeweeAzureSignalRNegotiateEndpoint();

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
