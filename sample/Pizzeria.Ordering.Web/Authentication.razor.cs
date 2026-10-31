using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace Pizzeria.Ordering.Web;

[SuppressMessage(
    "Design",
    "CA1515:Consider making public types internal",
    Justification = "Blazor components must be public to be rendered")]
public partial class Authentication
{
    [Parameter]
    public string Action { get; set; } = string.Empty;
}
