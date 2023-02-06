using Fluxor;
using Lewee.Fluxor;
using Sample.Restaurant.App.States.Tables.Actions;
using Sample.Restaurant.Application;

namespace Sample.Restaurant.App.States.Tables;

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
