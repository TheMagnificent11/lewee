using Fluxor;
using Lewee.Blazor.Fluxor;
using Sample.Pizzeria.Contracts;

namespace Sample.Pizzeria.Client.State.GetOrders;

public static class GetOrdersReducer
{
    [ReducerMethod]
    public static GetOrdersState OnGetOrders(GetOrdersState state, GetOrdersAction action) =>
        state.OnQuery<GetOrdersState, OrderDto[], GetOrdersAction>(action);

    [ReducerMethod]
    public static GetOrdersState OnGetOrdersSuccess(GetOrdersState state, GetOrdersSuccessAction action) =>
        state.OnQuerySuccess<GetOrdersState, OrderDto[], GetOrdersSuccessAction>(action);

    [ReducerMethod]
    public static GetOrdersState OnGetOrdersError(GetOrdersState state, GetOrdersErrorAction action) =>
        state.OnRequestError(action);
}
