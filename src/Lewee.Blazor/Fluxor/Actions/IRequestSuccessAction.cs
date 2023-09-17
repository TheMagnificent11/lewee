namespace Lewee.Blazor.Fluxor.Actions;

/// <summary>
/// Request Success Action Interface
/// </summary>
public interface IRequestSuccessAction
{
    /// <summary>
    /// Gets the request type
    /// </summary>
    string RequestType { get; }
}
