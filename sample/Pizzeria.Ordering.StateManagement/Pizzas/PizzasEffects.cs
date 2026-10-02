using System.Diagnostics.CodeAnalysis;
using Correlate;
using Fluxor;
using Lewee.Common;
using Lewee.Infrastructure.Fluxor;
using Microsoft.Extensions.Logging;
using Pizzeria.Ordering.StateManagement.Pizzas.Actions;
using Pizzeria.Store.Contracts.Pizzas;

namespace Pizzeria.Ordering.StateManagement.Pizzas;

public class PizzasEffects
    : QueryEffects<PizzasState, IEnumerable<PizzaDto>, LoadPizzasAction, LoadPizzasSuccessAction, LoadPizzasFailureAction>
{
    private readonly IBffApiClient bffApiClient;

    public PizzasEffects(
        IBffApiClient bffApiClient,
        IState<PizzasState> state,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<PizzasEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.bffApiClient = bffApiClient;
    }

    protected override async Task<QueryResult<IEnumerable<PizzaDto>>> ExecuteQueryAsync(
        [NotNull] LoadPizzasAction action,
        [NotNull] IDispatcher dispatcher)
    {
        var result = await this.bffApiClient.GetPizzasAsync();

        return QueryResult<IEnumerable<PizzaDto>>.Success(result);
    }
}
