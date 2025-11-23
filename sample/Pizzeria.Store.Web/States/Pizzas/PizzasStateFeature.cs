using System.Diagnostics.CodeAnalysis;
using Fluxor;

namespace Pizzeria.Store.Web.States.Pizzas;

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Used via Fluxor")]
internal sealed class PizzasStateFeature : Feature<PizzasState>
{
    public override string GetName() => nameof(PizzasState);

    protected override PizzasState GetInitialState() => new();
}
