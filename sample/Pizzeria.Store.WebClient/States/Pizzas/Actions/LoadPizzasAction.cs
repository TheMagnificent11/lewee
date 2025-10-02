using System.Diagnostics.CodeAnalysis;
using Pizzeria.Store.Contracts;

[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Related action types are grouped together in this file for better organization", Scope = "namespaceanddescendants", Target = "~N:Pizzeria.Store.WebClient.States.Pizzas.Actions")]

namespace Pizzeria.Store.WebClient.States.Pizzas.Actions;

public record LoadPizzasAction;

[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Related action types are grouped together for better organization")]
public record LoadPizzasSuccessAction(PizzaDto[] Pizzas);

[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Related action types are grouped together for better organization")]
public record LoadPizzasFailureAction(string ErrorMessage);
