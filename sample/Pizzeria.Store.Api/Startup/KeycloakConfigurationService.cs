using Pizzeria.Auth;

namespace Pizzeria.Store.Api.Startup;

internal sealed class KeycloakConfigurationService
{
    private readonly KeycloakHttpClient keycloakHttpClient;
    private readonly StartupStatusService startupStatusService;
    private readonly ILogger<KeycloakConfigurationService> logger;

    public KeycloakConfigurationService(
        KeycloakHttpClient keycloakHttpClient,
        StartupStatusService startupStatusService,
        ILogger<KeycloakConfigurationService> logger)
    {
        this.keycloakHttpClient = keycloakHttpClient;
        this.startupStatusService = startupStatusService;
        this.logger = logger;
    }

    public bool IsReady => this.startupStatusService.IsKeycloakReady;

    public async Task ConfigureAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Starting Keycloak configuration...");

        await this.keycloakHttpClient.WaitForReadyAsync(cancellationToken);

        var adminAccessToken = await this.keycloakHttpClient.GetAdminAccessTokenAsync(cancellationToken);

        this.keycloakHttpClient.SetBearerToken(adminAccessToken);

        await this.keycloakHttpClient.CreateRealmAsync(Pizzeria.Common.Environments.Auth.RealmName, cancellationToken);
        await this.keycloakHttpClient.CreateClientAsync(
            Pizzeria.Common.Environments.Auth.RealmName,
            Pizzeria.Common.Environments.Auth.ApiClientId,
            clientName: "Pizzeria Store API Client",
            cancellationToken);
        await this.keycloakHttpClient.CreateUserAsync(
            Pizzeria.Common.Environments.Auth.RealmName,
            Pizzeria.Common.Environments.Auth.Users.Customer1.Username,
            Pizzeria.Common.Environments.Auth.Users.Customer1.Password,
            cancellationToken);
        await this.keycloakHttpClient.CreateUserAsync(
            Pizzeria.Common.Environments.Auth.RealmName,
            Pizzeria.Common.Environments.Auth.Users.FrontStaff1.Username,
            Pizzeria.Common.Environments.Auth.Users.FrontStaff1.Password,
            cancellationToken);
        await this.keycloakHttpClient.CreateUserAsync(
            Pizzeria.Common.Environments.Auth.RealmName,
            Pizzeria.Common.Environments.Auth.Users.KitchenStaff1.Username,
            Pizzeria.Common.Environments.Auth.Users.KitchenStaff1.Password,
            cancellationToken);
        await this.keycloakHttpClient.CreateUserAsync(
            Pizzeria.Common.Environments.Auth.RealmName,
            Pizzeria.Common.Environments.Auth.Users.DeliveryDriver1.Username,
            Pizzeria.Common.Environments.Auth.Users.DeliveryDriver1.Password,
            cancellationToken);

        this.startupStatusService.SetKeycloakReady();

        this.logger.LogInformation("Keycloak configuration completed successfully");
    }
}
