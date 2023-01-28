using Fluxor;
using Sample.Restaurant.App.States.Tables.Actions;
using Sample.Restaurant.Application;

namespace Sample.Restaurant.App.States.Tables;

public static class TablesReducer
{
    [ReducerMethod]
    public static TablesState OnGetTables(TablesState state, GetTables action)
        => state with
        {
            IsLoading = true,
            CorrelationId = action.CorrelationId,
            Tables = Array.Empty<TableDto>(),
            ErrorMessage = null
        };

    [ReducerMethod]
    public static TablesState OnGetTablesSuccess(TablesState state, GetTablesSuccess action)
        => state with { IsLoading = false, Tables = action.Data };

    [ReducerMethod]
    public static TablesState OnGetTablesError(TablesState state, GetTablesError action)
        => state with { IsLoading = false, ErrorMessage = action.ErrorMessage };
}
