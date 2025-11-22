using System.Diagnostics.CodeAnalysis;
using Pizzeria.Store.Contracts;

namespace Pizzeria.Store.Web.States.Pizzas.Actions;

[SuppressMessage("Performance", "CA1515:Consider making public types internal", Justification = "Fluxor actions must be public")]
public record LoadPizzasSuccessAction(PizzaDto[] Pizzas);
