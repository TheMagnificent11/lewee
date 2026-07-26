using Fluxor;

namespace Pizzeria.Store.StateManagement.Pizzas;

public class PizzasStateFeature : Feature<PizzasState>
{
    public override string GetName() => nameof(PizzasState);

    protected override PizzasState GetInitialState() => new();
}
