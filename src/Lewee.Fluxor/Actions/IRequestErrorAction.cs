namespace Lewee.Fluxor.Actions;

/// <summary>
/// Request Error Action Interface
/// </summary>
public interface IRequestErrorAction
{
    /// <summary>
    /// Gets the error message
    /// </summary>
    string ErrorMessage { get; }
}
