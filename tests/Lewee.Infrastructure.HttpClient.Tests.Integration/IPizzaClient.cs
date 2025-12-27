using Lewee.Tests.Contracts;
using Refit;

namespace Lewee.Infrastructure.HttpClient.Tests.Integration;

internal interface IPizzaClient
{
    [Post("/api/pizzas")]
    Task AddPizzaToMenuAsync(AddPizzaToMenuRequest request, CancellationToken cancellationToken);

    [Get("/api/pizzas")]
    Task<IReadOnlyCollection<Pizza>> GetMenuAsync(CancellationToken cancellationToken);

    [Get("/api/pizzas/{id}")]
    Task<Pizza> GetPizzaAsync(Guid id, CancellationToken cancellationToken);
}
