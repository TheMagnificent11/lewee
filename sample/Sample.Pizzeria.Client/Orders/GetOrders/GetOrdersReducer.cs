using Fluxor;
using Lewee.Blazor.Fluxor;
using Sample.Pizzeria.Contracts;

namespace Sample.Pizzeria.Client.Orders.GetOrders;

public static class GetOrdersReducer
{
    [ReducerMethod]
    public static GetOrdersState OnGetOrdersAction(GetOrdersState state, GetOrdersAction action) =>
        state.OnQuery<GetOrdersState, OrderDto[], GetOrdersAction>(action);

    [ReducerMethod]
    public static GetOrdersState OnGetOrdersSuccessAction(GetOrdersState state, GetOrdersSuccessAction action) =>
        state.OnQuerySuccess<GetOrdersState, OrderDto[], GetOrdersSuccessAction>(action);

    [ReducerMethod]
    public static GetOrdersState OnGetOrdersErrorAction(GetOrdersState state, GetOrdersErrorAction action) =>
        state.OnRequestError(action);
}
