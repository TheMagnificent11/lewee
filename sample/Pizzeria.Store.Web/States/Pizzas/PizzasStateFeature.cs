using Fluxor;

namespace Pizzeria.Store.Web.States.Pizzas;

public sealed class PizzasStateFeature : Feature<PizzasState>
{
    public override string GetName() => nameof(PizzasState);

    protected override PizzasState GetInitialState() => new();
}
