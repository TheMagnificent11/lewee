using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace Pizzeria.Store.Web.Layout;

[SuppressMessage("Performance", "CA1515:Consider making public types internal", Justification = "Blazor component code-behind must be public to match generated partial class")]
public partial class MainLayout : LayoutComponentBase
{
    public const string SignOutButtonSelector = "button[aria-label='sign-out']";
}
