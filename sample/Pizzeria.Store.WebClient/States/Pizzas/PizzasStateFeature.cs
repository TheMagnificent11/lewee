using Fluxor;

namespace Pizzeria.Store.WebClient.States.Pizzas;

public sealed class PizzasStateFeature : Feature<PizzasState>
{
    public override string GetName() => nameof(PizzasState);

    protected override PizzasState GetInitialState() => new();
}
