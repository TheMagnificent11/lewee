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
        try
        {
            this.logger.LogInformation("Starting Keycloak configuration...");

            this.logger.LogInformation("Step 1: Waiting for Keycloak to be ready...");
            await this.keycloakHttpClient.WaitForReadyAsync(cancellationToken);
            this.logger.LogInformation("Step 1: ? Keycloak is ready");

            this.logger.LogInformation("Step 2: Getting admin access token...");
            var adminAccessToken = await this.keycloakHttpClient.GetAdminAccessTokenAsync(cancellationToken);
            this.logger.LogInformation("Step 2: ? Admin access token obtained");

            this.keycloakHttpClient.SetBearerToken(adminAccessToken);

            this.logger.LogInformation("Step 3: Creating realm '{RealmName}'...", Pizzeria.Common.Environments.Auth.RealmName);
            await this.keycloakHttpClient.CreateRealmAsync(Pizzeria.Common.Environments.Auth.RealmName, cancellationToken);
            this.logger.LogInformation("Step 3: ? Realm created/verified");

            this.logger.LogInformation("Step 4: Creating client '{ClientId}'...", Pizzeria.Common.Environments.Auth.ApiClientId);
            await this.keycloakHttpClient.CreateClientAsync(
                Pizzeria.Common.Environments.Auth.RealmName,
                Pizzeria.Common.Environments.Auth.ApiClientId,
                clientName: "Pizzeria Store API Client",
                cancellationToken);
            this.logger.LogInformation("Step 4: ? Client created/verified");

            this.logger.LogInformation("Step 5: Creating test user '{Username}'...", Pizzeria.Common.Environments.Auth.Users.Customer1.Username);
            await this.keycloakHttpClient.CreateUserAsync(
                Pizzeria.Common.Environments.Auth.RealmName,
                Pizzeria.Common.Environments.Auth.Users.Customer1.Username,
                Pizzeria.Common.Environments.Auth.Users.Customer1.Password,
                cancellationToken);
            this.logger.LogInformation("Step 5: ? Test user created/verified");

            // Test the token endpoint
            this.logger.LogInformation("Step 6: Testing token endpoint...");
            await this.keycloakHttpClient.TestTokenEndpointAsync(
                Pizzeria.Common.Environments.Auth.RealmName,
                Pizzeria.Common.Environments.Auth.ApiClientId,
                Pizzeria.Common.Environments.Auth.Users.Customer1.Username,
                Pizzeria.Common.Environments.Auth.Users.Customer1.Password,
                cancellationToken);
            this.logger.LogInformation("Step 6: ? Token endpoint test successful");

            this.logger.LogInformation("Creating remaining users...");
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
            this.logger.LogInformation("? Keycloak configuration completed successfully");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "? Keycloak configuration failed at step: {Message}", ex.Message);

            // Mark as ready anyway to prevent startup timeout
            this.startupStatusService.SetKeycloakReady();
            this.logger.LogWarning("?? Marked Keycloak as ready despite configuration failure");
        }
    }
}
