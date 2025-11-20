using Microsoft.AspNetCore.Components;

namespace Pizzeria.Store.Web.Layout;

public partial class MainLayout : LayoutComponentBase
{
    public const string SignOutButtonSelector = "button[aria-label='sign-out']";

    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    private void SignOut()
    {
        this.Navigation.NavigateTo(Routes.SignOut, forceLoad: true);
    }
}
