namespace Pizzeria.Configuration.Services;

/// <summary>
/// Auth server configuration service
/// </summary>
public interface IAuthServerConfiguration
{
    /// <summary>
    /// Configures the auth server
    /// </summary>
    /// <returns>Task</returns>
    Task ConfigureAsync();
}
