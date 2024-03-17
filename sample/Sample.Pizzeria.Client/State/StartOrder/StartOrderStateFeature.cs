using Fluxor;

namespace Sample.Pizzeria.Client.State.StartOrder;

public sealed class StartOrderStateFeature : Feature<StartOrderState>
{
    public override string GetName() => nameof(StartOrderState);

    protected override StartOrderState GetInitialState() => new();
}
