namespace Lewee.Domain;

/// <summary>
/// Client Service Interface
/// </summary>
public interface IClientService
{
    /// <summary>
    /// Gets the client ID that made the request in this request scope
    /// </summary>
    string? ClientId { get; }
}
