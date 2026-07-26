using System.Diagnostics.CodeAnalysis;
using Correlate;
using Fluxor;
using Lewee.Common;
using Lewee.Infrastructure.Fluxor;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Contracts.Pizzas;
using Pizzeria.Store.StateManagement.Pizzas.Actions;

namespace Pizzeria.Store.StateManagement.Pizzas;

public class PizzasEffects
    : QueryEffects<PizzasState, IEnumerable<PizzaDto>, LoadPizzasAction, LoadPizzasSuccessAction, LoadPizzasFailureAction>
{
    private readonly IStoreApiClient storeApiClient;

    public PizzasEffects(
        IStoreApiClient storeApiClient,
        IState<PizzasState> state,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<PizzasEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.storeApiClient = storeApiClient;
    }

    protected override async Task<QueryResult<IEnumerable<PizzaDto>>> ExecuteQueryAsync(
        [NotNull] LoadPizzasAction action,
        [NotNull] IDispatcher dispatcher)
    {
        var result = await this.storeApiClient.GetPizzasAsync();

        return QueryResult<IEnumerable<PizzaDto>>.Success(result);
    }
}
