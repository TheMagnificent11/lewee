using System.Diagnostics.CodeAnalysis;
using Correlate;
using Fluxor;
using Lewee.Common;
using Lewee.StateManagement;
using MediatR;
using Microsoft.Extensions.Logging;
using Pizzeria.Store.Application.Pizzas;
using Pizzeria.Store.Contracts.Pizzas;
using Pizzeria.Store.StateManagement.Pizzas.Actions;

namespace Pizzeria.Store.StateManagement.Pizzas;

public class PizzasEffects
    : QueryEffects<PizzasState, IEnumerable<PizzaDto>, LoadPizzasAction, LoadPizzasSuccessAction, LoadPizzasFailureAction>
{
    private readonly IMediator mediator;

    public PizzasEffects(
        IMediator mediator,
        IState<PizzasState> state,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger<PizzasEffects> logger)
        : base(state, correlationContextAccessor, logger)
    {
        this.mediator = mediator;
    }

    protected override async Task<QueryResult<IEnumerable<PizzaDto>>> ExecuteQueryAsync([NotNull] LoadPizzasAction action, [NotNull] IDispatcher dispatcher)
    {
        return await this.mediator.Send(new GetPizzasQuery(action.CorrelationId));
    }
}
