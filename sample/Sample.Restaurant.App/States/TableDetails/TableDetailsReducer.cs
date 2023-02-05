using Fluxor;
using Lewee.Fluxor;
using Sample.Restaurant.App.States.TableDetails.Actions;
using Sample.Restaurant.Application;

namespace Sample.Restaurant.App.States.TableDetails;

internal static class TableDetailsReducer
{
    [ReducerMethod]
    public static TableDetailsState OnGetTableDetails(TableDetailsState state, GetTableDetails action)
        => state.OnQuery<TableDetailsState, TableDetailsDto, GetTableDetails>(action);

    [ReducerMethod]
    public static TableDetailsState OnGetTableDetailsSuccess(TableDetailsState state, GetTableDetailsSuccess action)
        => state.OnQuerySuccess<TableDetailsState, TableDetailsDto, GetTableDetailsSuccess>(action);

    [ReducerMethod]
    public static TableDetailsState OnGetTableDetailsError(TableDetailsState state, GetTableDetailsError action)
        => state.OnRequestError(action);
}
