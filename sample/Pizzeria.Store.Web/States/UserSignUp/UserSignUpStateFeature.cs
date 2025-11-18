using Fluxor;

namespace Pizzeria.Store.Web.States.UserSignUp;

public sealed class UserSignUpStateFeature : Feature<UserSignUpState>
{
    public override string GetName() => nameof(UserSignUpState);

    protected override UserSignUpState GetInitialState() => new();
}
