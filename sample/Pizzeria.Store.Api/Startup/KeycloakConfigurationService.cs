using Pizzeria.Auth;
using PizzeriaEnvironment = Pizzeria.Common.Environments;

namespace Pizzeria.Store.Api.Startup;

internal sealed class KeycloakConfigurationService
{
    private readonly KeycloakHttpClient keycloakHttpClient;
    private readonly ILogger<KeycloakConfigurationService> logger;

    public KeycloakConfigurationService(
        KeycloakHttpClient keycloakHttpClient,
        ILogger<KeycloakConfigurationService> logger)
    {
        this.keycloakHttpClient = keycloakHttpClient;
        this.logger = logger;
    }

    public async Task ConfigureAsync()
    {
        this.logger.LogInformation("Starting Keycloak configuration...");

        await this.keycloakHttpClient.WaitForReadyAsync();

        var adminAccessToken = await this.keycloakHttpClient.GetAdminAccessTokenAsync();

        this.keycloakHttpClient.SetBearerToken(adminAccessToken);

        await this.keycloakHttpClient.CreateRealmAsync(PizzeriaEnvironment.Auth.RealmName);
        await this.keycloakHttpClient.CreateClientAsync(
            PizzeriaEnvironment.Auth.RealmName,
            PizzeriaEnvironment.Auth.ApiClientId,
            clientName: "Pizzeria Store API Client");
        await this.keycloakHttpClient.CreateUserAsync(
            PizzeriaEnvironment.Auth.RealmName,
            PizzeriaEnvironment.Auth.Users.Customer1.Username,
            PizzeriaEnvironment.Auth.Users.Customer1.Password);
        await this.keycloakHttpClient.CreateUserAsync(
            PizzeriaEnvironment.Auth.RealmName,
            PizzeriaEnvironment.Auth.Users.FrontStaff1.Username,
            PizzeriaEnvironment.Auth.Users.FrontStaff1.Password);
        await this.keycloakHttpClient.CreateUserAsync(
            PizzeriaEnvironment.Auth.RealmName,
            PizzeriaEnvironment.Auth.Users.KitchenStaff1.Username,
            PizzeriaEnvironment.Auth.Users.KitchenStaff1.Password);
        await this.keycloakHttpClient.CreateUserAsync(
            PizzeriaEnvironment.Auth.RealmName,
            PizzeriaEnvironment.Auth.Users.DeliveryDriver1.Username,
            PizzeriaEnvironment.Auth.Users.DeliveryDriver1.Password);

        this.logger.LogInformation("Keycloak configuration completed successfully");
    }
}
