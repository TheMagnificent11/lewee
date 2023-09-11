namespace Lewee.Blazor.Fluxor.Actions;

/// <summary>
/// Request Error Action Interface
/// </summary>
public interface IRequestErrorAction
{
    /// <summary>
    /// Gets the request type
    /// </summary>
    string RequestType { get; }

    /// <summary>
    /// Gets the error message
    /// </summary>
    string ErrorMessage { get; }
}
