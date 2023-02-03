using Lewee.Fluxor.Actions;
using Sample.Restaurant.Application;

namespace Sample.Restaurant.App.States.TableDetails.Actions;

public record GetTableDetailsSuccess : IQuerySuccessAction<TableDetailsDto>
{
    public GetTableDetailsSuccess(TableDetailsDto data)
    {
        this.Data = data;
    }

    public TableDetailsDto Data { get; }
}
