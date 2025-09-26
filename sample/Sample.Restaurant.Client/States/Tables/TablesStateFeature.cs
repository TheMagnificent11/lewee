using Fluxor;

namespace Sample.Restaurant.Client.States.Tables;

public sealed class TablesStateFeature : Feature<TablesState>
{
    public override string GetName() => nameof(TablesState);

    protected override TablesState GetInitialState() => new();
}
