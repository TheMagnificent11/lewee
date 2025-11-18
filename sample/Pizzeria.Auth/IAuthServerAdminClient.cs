namespace Pizzeria.Auth;

public interface IAuthServerAdminClient
{
    Task<bool> ClientExistsAsync(string realmName, string clientId, CancellationToken cancellationToken);

    Task CreateClientAsync(string realmName, string clientId, string clientName, CancellationToken cancellationToken);

    Task DeleteClientAsync(string realmName, string clientId, CancellationToken cancellationToken);

    Task<bool> UserExistsAsync(string realmName, string username, CancellationToken cancellationToken);

    Task CreateUserAsync(string realmName, string username, string password, CancellationToken cancellationToken);

    Task<string> GetUserIdAsync(string realmName, string username, CancellationToken cancellationToken);

    Task WaitForReadyAsync(CancellationToken cancellationToken);

    Task<string> TestTokenEndpointAsync(
        string realmName,
        string clientId,
        string username,
        string password,
        CancellationToken cancellationToken);
}
