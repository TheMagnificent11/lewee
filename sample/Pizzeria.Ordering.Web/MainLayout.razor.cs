using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace Pizzeria.Ordering.Web;

[SuppressMessage(
    "Design",
    "CA1515:Consider making public types internal",
    Justification = "Blazor components must be public to be rendered")]
public partial class MainLayout : LayoutComponentBase
{
    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    private void SignOut()
    {
        var signOutUri = new Uri(new Uri(this.Navigation.BaseUri), "authentication/logout");
        this.Navigation.NavigateTo(signOutUri.ToString());
    }

    [SuppressMessage(
        "SonarAnalyzer.CSharp",
        "S1144:Unused private types or members should be removed",
        Justification = "Used by integration tests")]
    public static class Selectors
    {
        public const string BannerHeading = "[role='heading'][aria-level='1']";
        public const string SignOutButton = $"button[aria-label='{AriaLabels.SignOut}']";
    }

    internal static class AriaLabels
    {
        public const string SignOut = "sign-out";
    }
}
