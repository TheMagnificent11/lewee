using Fluxor;

namespace Sample.Restaurant.App.States.Tables;

public sealed class TablesFeatureState : Feature<TablesState>
{
    public override string GetName() => nameof(TablesState);

    protected override TablesState GetInitialState() => new();
}