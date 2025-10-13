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

    public bool IsReady { get; private set; }

    public async Task ConfigureAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Starting Keycloak configuration...");

        await this.keycloakHttpClient.WaitForReadyAsync(cancellationToken);

        var adminAccessToken = await this.keycloakHttpClient.GetAdminAccessTokenAsync(cancellationToken);

        this.keycloakHttpClient.SetBearerToken(adminAccessToken);

        await this.keycloakHttpClient.CreateRealmAsync(PizzeriaEnvironment.Auth.RealmName, cancellationToken);
        await this.keycloakHttpClient.CreateClientAsync(
            PizzeriaEnvironment.Auth.RealmName,
            PizzeriaEnvironment.Auth.ApiClientId,
            clientName: "Pizzeria Store API Client",
            cancellationToken);
        await this.keycloakHttpClient.CreateUserAsync(
            PizzeriaEnvironment.Auth.RealmName,
            PizzeriaEnvironment.Auth.Users.Customer1.Username,
            PizzeriaEnvironment.Auth.Users.Customer1.Password,
            cancellationToken);
        await this.keycloakHttpClient.CreateUserAsync(
            PizzeriaEnvironment.Auth.RealmName,
            PizzeriaEnvironment.Auth.Users.FrontStaff1.Username,
            PizzeriaEnvironment.Auth.Users.FrontStaff1.Password,
            cancellationToken);
        await this.keycloakHttpClient.CreateUserAsync(
            PizzeriaEnvironment.Auth.RealmName,
            PizzeriaEnvironment.Auth.Users.KitchenStaff1.Username,
            PizzeriaEnvironment.Auth.Users.KitchenStaff1.Password,
            cancellationToken);
        await this.keycloakHttpClient.CreateUserAsync(
            PizzeriaEnvironment.Auth.RealmName,
            PizzeriaEnvironment.Auth.Users.DeliveryDriver1.Username,
            PizzeriaEnvironment.Auth.Users.DeliveryDriver1.Password,
            cancellationToken);

        this.IsReady = true;

        this.logger.LogInformation("Keycloak configuration completed successfully");
    }
}
