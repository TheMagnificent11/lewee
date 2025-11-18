using System.Diagnostics.CodeAnalysis;
using Fluxor;
using Pizzeria.Store.Web.Services;
using Pizzeria.Store.Web.States.UserSignUp.Actions;

namespace Pizzeria.Store.Web.States.UserSignUp;

public class UserSignUpEffects
{
    private readonly IPizzeriaApiClient apiClient;

    public UserSignUpEffects(IPizzeriaApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    [EffectMethod]
    public async Task OnSignUpAsync(SignUpAction action, IDispatcher dispatcher)
    {
        try
        {
            var request = new CreateCustomerRequest(action.Username, action.Password);
            await this.apiClient.CreateCustomerAsync(request);
            dispatcher.Dispatch(new SignUpSuccessAction());
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new SignUpFailureAction($"Failed to sign up: {ex.Message}"));
        }
    }
}
