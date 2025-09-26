using Fluxor;
using Lewee.Blazor.Fluxor;
using Sample.Restaurant.Client.States.UseTable.Actions;

namespace Sample.Restaurant.Client.States.UseTable;

public static class UseTableReducer
{
    [ReducerMethod]
    public static UseTableState OnUseTable(UseTableState state, UseTableAction action)
        => state.OnRequest(action) with { TableNumber = action.TableNumber };

    [ReducerMethod]
    public static UseTableState OnUseTableError(UseTableState state, UseTableErrorAction action)
        => state.OnRequestError(action);
}
