namespace Lewee.Auth.Api;

/// <summary>
/// Request to provision a user.
/// </summary>
public sealed record CreateUserRequest
{
    /// <summary>
    /// Gets the external user identity.
    /// </summary>
    public string ExternalUserId { get; init; }
}
