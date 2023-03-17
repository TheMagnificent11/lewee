using Fluxor;

namespace Sample.Restaurant.Client.States.UseTable;

public sealed class UseTableFeatureState : Feature<UseTableState>
{
    public override string GetName() => nameof(UseTableState);

    protected override UseTableState GetInitialState() => new();
}
