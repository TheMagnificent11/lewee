namespace Lewee.Tests.Contracts;

public record Pizza(
    Guid Id,
    string Name,
    string Description,
    decimal Price);
