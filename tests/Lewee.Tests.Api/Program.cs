using System.Collections.Concurrent;
using Lewee.Tests.Contracts;
using Lewee.Tests.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapDefaultEndpoints();

var pizzas = new ConcurrentDictionary<string, Pizza>(StringComparer.OrdinalIgnoreCase);

app.MapPost(Endpoints.Pizzas, (AddPizzaToMenuRequest request) =>
{
    if (pizzas.ContainsKey(request.Name))
    {
        return Results.BadRequest("Pizza already exists");
    }

    var pizza = new Pizza(request.Name, request.Price);

    if (pizzas.TryAdd(request.Name, pizza))
    {
        return Results.Ok();
    }

    return Results.InternalServerError("Unexpected error occurred");
});

app.MapGet(Endpoints.Pizzas, () =>
{
    var result = pizzas.ToArray();

    return Results.Ok(result);
});

await app.RunAsync();
