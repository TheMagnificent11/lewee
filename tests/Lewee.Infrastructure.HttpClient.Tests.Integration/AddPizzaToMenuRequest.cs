namespace Lewee.Infrastructure.HttpClient.Tests.Integration;

internal record AddPizzaToMenuRequest(
    string Name,
    string Description,
    decimal Price);
