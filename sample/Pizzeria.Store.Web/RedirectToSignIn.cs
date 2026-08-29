using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace Pizzeria.Store.Web;

[SuppressMessage(
    "Design",
    "CA1515:Consider making public types internal",
    Justification = "Blazor components must be public to be rendered")]
public sealed class RedirectToSignIn : ComponentBase
{
    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    protected override void OnInitialized()
    {
        var relativePath = this.Navigation.ToBaseRelativePath(this.Navigation.Uri);
        var returnUrl = Uri.EscapeDataString($"/{relativePath}");
        this.Navigation.NavigateTo($"/authentication/sign-in?returnUrl={returnUrl}", forceLoad: true);
    }
}
