using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Pizzeria.Store.Web.States.UserSignUp;
using Pizzeria.Store.Web.States.UserSignUp.Actions;

namespace Pizzeria.Store.Web.Pages;

public partial class SignUp : FluxorComponent, IDisposable
{
    private string username = string.Empty;
    private string password = string.Empty;

    [Inject]
    private IState<UserSignUpState> UserSignUpState { get; set; } = null!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    public void Dispose()
    {
        this.UserSignUpState.StateChanged -= this.OnStateChanged;
        GC.SuppressFinalize(this);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.UserSignUpState.StateChanged += this.OnStateChanged;
    }

    private void OnStateChanged(object? sender, EventArgs e)
    {
        if (this.UserSignUpState.Value.IsSuccess)
        {
            this.NavigationManager.NavigateTo(Routes.Order);
        }
    }

    private void HandleSignUp()
    {
        if (string.IsNullOrWhiteSpace(this.username) || string.IsNullOrWhiteSpace(this.password))
        {
            return;
        }

        this.Dispatcher.Dispatch(new SignUpAction(this.username, this.password));
    }
}
