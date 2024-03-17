using Fluxor;
using Lewee.Blazor.Fluxor;

namespace Sample.Pizzeria.Client.State.StartOrder;

public static class StartOrderReducer
{
    [ReducerMethod]
    public static StartOrderState OnStartOrder(StartOrderState state, StartOrderAction action) =>
        state.OnRequest(action);

    [ReducerMethod]
    public static StartOrderState OnStartOrderError(StartOrderState state, StartOrderErrorAction action) =>
        state.OnRequestError(action);
}
