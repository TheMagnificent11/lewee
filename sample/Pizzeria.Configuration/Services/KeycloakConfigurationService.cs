using Microsoft.Extensions.Logging;
using Pizzeria.Auth;
using Pizzeria.Common;

namespace Pizzeria.Configuration.Services;

/// <summary>
/// Keycloak auth server configuration service
/// </summary>
public sealed class KeycloakConfigurationService : IAuthServerConfiguration
{
    private readonly KeycloakHttpClient keycloakHttpClient;
    private readonly ILogger<KeycloakConfigurationService> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeycloakConfigurationService"/> class.
    /// </summary>
    /// <param name="keycloakHttpClient">Keycloak HTTP client</param>
    /// <param name="logger">Logger</param>
    public KeycloakConfigurationService(
        KeycloakHttpClient keycloakHttpClient,
        ILogger<KeycloakConfigurationService> logger)
    {
        this.keycloakHttpClient = keycloakHttpClient;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public async Task ConfigureAsync()
    {
        this.logger.LogInformation("Starting Keycloak configuration...");

        await this.keycloakHttpClient.WaitForReadyAsync();

        var adminAccessToken = await this.keycloakHttpClient.GetAdminAccessTokenAsync();

        this.keycloakHttpClient.SetBearerToken(adminAccessToken);

        await this.keycloakHttpClient.CreateRealmAsync(Environments.Auth.RealmName);
        await this.keycloakHttpClient.CreateClientAsync(Environments.Auth.RealmName, Environments.Auth.ApiClientId);
        await this.keycloakHttpClient.CreateUserAsync(
            Environments.Auth.RealmName,
            Environments.Auth.Users.Customer1.Username,
            Environments.Auth.Users.Customer1.Password);
        await this.keycloakHttpClient.CreateUserAsync(
            Environments.Auth.RealmName,
            Environments.Auth.Users.FrontStaff1.Username,
            Environments.Auth.Users.FrontStaff1.Password);
        await this.keycloakHttpClient.CreateUserAsync(
            Environments.Auth.RealmName,
            Environments.Auth.Users.KitchenStaff1.Username,
            Environments.Auth.Users.KitchenStaff1.Password);
        await this.keycloakHttpClient.CreateUserAsync(
            Environments.Auth.RealmName,
            Environments.Auth.Users.DeliveryDriver1.Username,
            Environments.Auth.Users.DeliveryDriver1.Password);

        this.logger.LogInformation("Keycloak configuration completed successfully");
    }
}
