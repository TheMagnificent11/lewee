using Microsoft.AspNetCore.Components;

namespace Pizzeria.Store.Web;

public partial class MainLayout : LayoutComponentBase
{
    public static class Selectors
    {
        public const string BannerHeading = "[role='heading'][aria-level='1']";
        public const string SignOutButton = $"button[aria-label='{AriaLabels.SignOut}']";
    }

    private static class AriaLabels
    {
        public const string SignOut = "sign-out";
    }
}
