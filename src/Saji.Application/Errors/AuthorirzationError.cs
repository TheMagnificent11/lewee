namespace Saji.Application.Errors;

/// <summary>
/// Authorization Error
/// </summary>
public class AuthorirzationError : BaseError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorirzationError"/> class
    /// </summary>
    public AuthorirzationError()
        : base("The authenticated user is not authorized to perform this action")
    {
    }
}
