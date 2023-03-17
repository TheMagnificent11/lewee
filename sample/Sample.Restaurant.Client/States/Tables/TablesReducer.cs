using Fluxor;
using Lewee.Blazor.Fluxor;
using Sample.Restaurant.Client.States.Tables.Actions;

namespace Sample.Restaurant.Client.States.Tables;

public static class TablesReducer
{
    [ReducerMethod]
    public static TablesState OnGetTables(TablesState state, GetTablesAction action)
        => state.OnQuery<TablesState, TableDto[], GetTablesAction>(action);

    [ReducerMethod]
    public static TablesState OnGetTablesSuccess(TablesState state, GetTablesSuccessAction action)
        => state.OnQuerySuccess<TablesState, TableDto[], GetTablesSuccessAction>(action);

    [ReducerMethod]
    public static TablesState OnGetTablesError(TablesState state, GetTablesErrorAction action)
        => state.OnRequestError(action);
}
