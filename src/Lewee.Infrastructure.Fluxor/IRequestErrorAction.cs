namespace Lewee.Infrastructure.Fluxor;

/// <summary>
/// Request Error Action Interface
/// </summary>
public interface IRequestErrorAction : IRequestAction
{
    /// <summary>
    /// Gets the error message
    /// </summary>
    string ErrorMessage { get; init; }
}
