using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Related action types are grouped together in this file for better organization", Scope = "namespaceanddescendants", Target = "~N:Pizzeria.Store.WebClient.States.Orders.Actions")]

namespace Pizzeria.Store.WebClient.States.Orders.Actions;

public record StartOrderAction;

[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Related action types are grouped together for better organization")]
public record StartOrderSuccessAction(Guid OrderId);

[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Related action types are grouped together for better organization")]
public record StartOrderFailureAction(string ErrorMessage);

[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Related action types are grouped together for better organization")]
public record AddPizzaToOrderAction(Guid OrderId, Guid PizzaId);

[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Related action types are grouped together for better organization")]
public record AddPizzaToOrderSuccessAction(Guid PizzaId);

[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Related action types are grouped together for better organization")]
public record AddPizzaToOrderFailureAction(Guid PizzaId, string ErrorMessage);

[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Related action types are grouped together for better organization")]
public record ClearOrderErrorAction;
