using Fluxor;

namespace Sample.Restaurant.App.States.TableDetails;

public sealed class TableDetailsFeatureState : Feature<TableDetailsState>
{
    public override string GetName() => nameof(TableDetailsState);

    protected override TableDetailsState GetInitialState() => new();
}
