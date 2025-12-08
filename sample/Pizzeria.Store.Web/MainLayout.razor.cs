using Microsoft.AspNetCore.Components;

namespace Pizzeria.Store.Web;

public partial class MainLayout : LayoutComponentBase
{
    public const string BannerHeading = "[role='heading'][aria-level='1']";
    public const string SignOutButtonSelector = "button[aria-label='sign-out']";
}
