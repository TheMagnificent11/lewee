using System.Diagnostics.CodeAnalysis;
using Fluxor;
using Pizzeria.Store.Web.States.UserSignUp.Actions;

namespace Pizzeria.Store.Web.States.UserSignUp;

public static class UserSignUpReducer
{
    [ReducerMethod]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter '_' should begin with lower-case letter", Justification = "Underscore is the standard discard pattern for unused parameters")]
    public static UserSignUpState OnSignUp(UserSignUpState state, SignUpAction _)
    {
        return state with
        {
            IsSigningUp = true,
            IsSuccess = false,
            ErrorMessage = null
        };
    }

    [ReducerMethod]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter '_' should begin with lower-case letter", Justification = "Underscore is the standard discard pattern for unused parameters")]
    public static UserSignUpState OnSignUpSuccess(UserSignUpState state, SignUpSuccessAction _)
    {
        return state with
        {
            IsSigningUp = false,
            IsSuccess = true,
            ErrorMessage = null
        };
    }

    [ReducerMethod]
    public static UserSignUpState OnSignUpFailure(UserSignUpState state, SignUpFailureAction action)
    {
        return state with
        {
            IsSigningUp = false,
            IsSuccess = false,
            ErrorMessage = action.ErrorMessage
        };
    }
}
