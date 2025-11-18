namespace Pizzeria.Auth;

/// <summary>
/// Interface for authentication server admin operations
/// </summary>
public interface IAuthServerAdminClient
{
    /// <summary>
    /// Checks if a client exists in a realm
    /// </summary>
    /// <param name="realmName">Name of the realm</param>
    /// <param name="clientId">Client ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the client exists, false otherwise</returns>
    Task<bool> ClientExistsAsync(string realmName, string clientId, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new client in a realm
    /// </summary>
    /// <param name="realmName">Name of the realm</param>
    /// <param name="clientId">Client ID</param>
    /// <param name="clientName">Client name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task CreateClientAsync(string realmName, string clientId, string clientName, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a client from a realm
    /// </summary>
    /// <param name="realmName">Name of the realm</param>
    /// <param name="clientId">Client ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteClientAsync(string realmName, string clientId, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if a user exists in a realm
    /// </summary>
    /// <param name="realmName">Name of the realm</param>
    /// <param name="username">Username</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the user exists, false otherwise</returns>
    Task<bool> UserExistsAsync(string realmName, string username, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new user in a realm
    /// </summary>
    /// <param name="realmName">Name of the realm</param>
    /// <param name="username">Username</param>
    /// <param name="password">Password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task CreateUserAsync(string realmName, string username, string password, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the user ID for a username in a realm
    /// </summary>
    /// <param name="realmName">Name of the realm</param>
    /// <param name="username">Username</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User ID</returns>
    Task<string> GetUserIdAsync(string realmName, string username, CancellationToken cancellationToken);

    /// <summary>
    /// Waits for the authentication server to be ready
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    Task WaitForReadyAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Tests the token endpoint for a realm and client
    /// </summary>
    /// <param name="realmName">Name of the realm</param>
    /// <param name="clientId">Client ID</param>
    /// <param name="username">Username</param>
    /// <param name="password">Password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Access token</returns>
    Task<string> TestTokenEndpointAsync(
        string realmName,
        string clientId,
        string username,
        string password,
        CancellationToken cancellationToken);
}
