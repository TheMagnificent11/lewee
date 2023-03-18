namespace Lewee.Domain;

/// <summary>
/// Authenticated User Service Interface
/// </summary>
public interface IAuthenticatedUserService
{
    /// <summary>
    /// Gets the user ID of the authenticated user if a user is authenticated, otherwise return null
    /// </summary>
    string? UserId { get; }
}
