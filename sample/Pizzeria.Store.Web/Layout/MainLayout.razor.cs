using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Pizzeria.Store.Web.Layout;

public partial class MainLayout
{
    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    private void SignOut()
    {
        this.Navigation.NavigateTo("/logout", forceLoad: true);
    }
}
