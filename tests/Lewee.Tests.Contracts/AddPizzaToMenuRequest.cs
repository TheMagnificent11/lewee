namespace Lewee.Tests.Contracts;

public record AddPizzaToMenuRequest(
    string Name,
    string Description,
    decimal Price);
