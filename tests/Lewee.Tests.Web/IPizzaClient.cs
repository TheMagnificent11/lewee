namespace Lewee.Tests.Web;

internal interface IPizzaClient
{
    Task AddPizzaToMenuAsync(AddPizzaToMenuRequest request, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Pizza>> GetMenuAsync(CancellationToken cancellationToken);

    Task<Pizza> GetPizzaAsync(Guid id, CancellationToken cancellationToken);
}
