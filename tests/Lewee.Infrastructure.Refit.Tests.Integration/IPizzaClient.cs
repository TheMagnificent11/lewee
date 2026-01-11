using Lewee.Tests.Contracts;
using Refit;

namespace Lewee.Infrastructure.Refit.Tests.Integration;

internal interface IPizzaClient
{
    [Post(Endpoints.Pizzas)]
    Task AddPizzaToMenuAsync(AddPizzaToMenuRequest request, CancellationToken cancellationToken);

    [Get(Endpoints.Pizzas)]
    Task<IReadOnlyCollection<Pizza>> GetMenuAsync(CancellationToken cancellationToken);
}
