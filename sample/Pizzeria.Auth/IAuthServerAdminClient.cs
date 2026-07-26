namespace Pizzeria.Auth;

public interface IAuthServerAdminClient
{
    Task<bool> UserExistsAsync(string username, CancellationToken cancellationToken);

    Task CreateUserAsync(string username, string password, CancellationToken cancellationToken);

    Task<string> GetUserIdAsync(string username, CancellationToken cancellationToken);
}
