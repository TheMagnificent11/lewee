using Fluxor;
using Lewee.Fluxor;
using Sample.Restaurant.App.States.Tables.Actions;
using Sample.Restaurant.Application;

namespace Sample.Restaurant.App.States.Tables;

public static class TablesReducer
{
    [ReducerMethod]
    public static TablesState OnGetTables(TablesState state, GetTables action)
        => state.OnQuery<TablesState, TableDto[], GetTables>(action);

    [ReducerMethod]
    public static TablesState OnGetTablesSuccess(TablesState state, GetTablesSuccess action)
        => state.OnQuerySuccess<TablesState, TableDto[], GetTablesSuccess>(action);

    [ReducerMethod]
    public static TablesState OnGetTablesError(TablesState state, GetTablesError action)
        => state.OnRequestError(action);
}
