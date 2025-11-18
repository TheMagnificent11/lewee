namespace Pizzeria.Store.Web.States.UserSignUp;

public record UserSignUpState
{
    public bool IsSigningUp { get; init; } = false;
    public bool IsSuccess { get; init; } = false;
    public string? ErrorMessage { get; init; }
}
