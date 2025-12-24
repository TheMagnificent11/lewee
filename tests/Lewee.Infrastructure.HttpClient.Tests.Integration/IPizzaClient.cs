namespace Lewee.Infrastructure.HttpClient.Tests.Integration;

internal interface IPizzaClient
{
    Task AddPizzaToMenuAsync(AddPizzaToMenuRequest request, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Pizza>> GetMenuAsync(CancellationToken cancellationToken);

    Task<Pizza> GetPizzaAsync(Guid id, CancellationToken cancellationToken);
}
