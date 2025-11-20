using Microsoft.AspNetCore.Components;

namespace Pizzeria.Store.Web.Layout;

public partial class MainLayout : LayoutComponentBase
{
    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    private void SignOut()
    {
        this.Navigation.NavigateTo("/logout", forceLoad: true);
    }
}
