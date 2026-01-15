using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace Pizzeria.Store.Web;

[SuppressMessage(
    "Design",
    "CA1515:Consider making public types internal",
    Justification = "Blazor components must be public to be rendered")]
public sealed class RedirectToLogin : ComponentBase
{
    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    protected override void OnInitialized()
    {
        var returnUrl = Uri.EscapeDataString(this.Navigation.Uri);
        this.Navigation.NavigateTo($"authentication/login?returnUrl={returnUrl}", forceLoad: true);
    }
}
