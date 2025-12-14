using System.Diagnostics.CodeAnalysis;
using Correlate;
using Fluxor;
using Lewee.StateManagement.Observability;
using Microsoft.Extensions.Logging;

namespace Lewee.StateManagement;

/// <summary>
/// Request Effects
/// </summary>
/// <typeparam name="TState">State type</typeparam>
/// <typeparam name="TData">Data type</typeparam>
/// <typeparam name="TRequestAction">Request action type</typeparam>
/// <typeparam name="TRequestSuccessAction">Request success action type</typeparam>
/// <typeparam name="TRequestErrorAction">Request error action type</typeparam>
public abstract class QuerytEffects<TState, TData, TRequestAction, TRequestSuccessAction, TRequestErrorAction>
    : RequestEffects<TState, TData, TRequestAction, TRequestSuccessAction, TRequestErrorAction>
    where TState : RequestState<TData>, new()
    where TData : class
    where TRequestAction : IRequestAction, new()
    where TRequestSuccessAction : IQuerySuccessAction<TData>, new()
    where TRequestErrorAction : IRequestErrorAction, new()
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="QuerytEffects{TState, TData, TRequestAction, TRequestSuccessAction, TRequestErrorAction}"/> class
    /// </summary>
    /// <param name="state">State</param>
    /// <param name="correlationContextAccessor">Correlation context accessor</param>
    /// <param name="logger">Logger</param>
    protected QuerytEffects(
        IState<TState> state,
        ICorrelationContextAccessor correlationContextAccessor,
        ILogger logger)
        : base(state, correlationContextAccessor, logger)
    {
    }
}
