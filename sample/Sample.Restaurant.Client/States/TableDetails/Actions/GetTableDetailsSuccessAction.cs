using Lewee.Blazor.Fluxor.Actions;

namespace Sample.Restaurant.Client.States.TableDetails.Actions;

public record GetTableDetailsSuccessAction : IQuerySuccessAction<TableDetailsDto>
{
    public GetTableDetailsSuccessAction(TableDetailsDto data)
    {
        this.Data = data;
    }

    public TableDetailsDto Data { get; }

    public string RequestType => "GetTableDetails";
}
