using System.Diagnostics.CodeAnalysis;
using Lewee.Blazor.Fluxor.Actions;

namespace Pizzeria.Store.Web.States.Orders.Actions;

[SuppressMessage("Performance", "CA1515:Consider making public types internal", Justification = "Fluxor actions must be public")]
public record StartOrderFailureAction : IRequestErrorAction
{
    public StartOrderFailureAction(Guid correlationId, string errorMessage)
    {
        this.CorrelationId = correlationId;
        this.ErrorMessage = errorMessage;
    }

    public Guid CorrelationId { get; init; }
    public string ErrorMessage { get; init; }
}
