using Pizzeria.Auth;
using PizzeriaEnvironments = Pizzeria.Common.Environments;

namespace Pizzeria.Configuration;

internal sealed class AuthServerConfigurationService
{
    private readonly IAuthServerAdminClient authServerAdminClient;
    private readonly ILogger<AuthServerConfigurationService> logger;

    public AuthServerConfigurationService(
        IAuthServerAdminClient authServerAdminClient,
        ILogger<AuthServerConfigurationService> logger)
    {
        this.authServerAdminClient = authServerAdminClient;
        this.logger = logger;
    }

    public async Task ConfigureAsync(CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Starting Auth Server configuration...");

            this.logger.LogInformation("Step 1: Waiting for Auth Server to be ready...");
            await this.authServerAdminClient.WaitForReadyAsync(cancellationToken);
            this.logger.LogInformation("Step 1: Auth Server is ready");

            // Note: Realm and client are now imported via JSON in AppHost
            // We only need to create users here
            this.logger.LogInformation("Step 2: Creating test users...");

            this.logger.LogInformation("Creating user '{Username}'...", PizzeriaEnvironments.Auth.Users.Customer1.Username);
            await this.authServerAdminClient.CreateUserAsync(
                PizzeriaEnvironments.Auth.RealmName,
                PizzeriaEnvironments.Auth.Users.Customer1.Username,
                PizzeriaEnvironments.Auth.Users.Customer1.Password,
                cancellationToken);

            this.logger.LogInformation("Creating user '{Username}'...", PizzeriaEnvironments.Auth.Users.FrontStaff1.Username);
            await this.authServerAdminClient.CreateUserAsync(
                PizzeriaEnvironments.Auth.RealmName,
                PizzeriaEnvironments.Auth.Users.FrontStaff1.Username,
                PizzeriaEnvironments.Auth.Users.FrontStaff1.Password,
                cancellationToken);

            this.logger.LogInformation("Creating user '{Username}'...", PizzeriaEnvironments.Auth.Users.KitchenStaff1.Username);
            await this.authServerAdminClient.CreateUserAsync(
                PizzeriaEnvironments.Auth.RealmName,
                PizzeriaEnvironments.Auth.Users.KitchenStaff1.Username,
                PizzeriaEnvironments.Auth.Users.KitchenStaff1.Password,
                cancellationToken);

            this.logger.LogInformation("Creating user '{Username}'...", PizzeriaEnvironments.Auth.Users.DeliveryDriver1.Username);
            await this.authServerAdminClient.CreateUserAsync(
                PizzeriaEnvironments.Auth.RealmName,
                PizzeriaEnvironments.Auth.Users.DeliveryDriver1.Username,
                PizzeriaEnvironments.Auth.Users.DeliveryDriver1.Password,
                cancellationToken);

            this.logger.LogInformation("Step 2: All test users created/verified");

            // Test the token endpoint
            this.logger.LogInformation("Step 3: Testing token endpoint...");
            await this.authServerAdminClient.TestTokenEndpointAsync(
                PizzeriaEnvironments.Auth.RealmName,
                PizzeriaEnvironments.Auth.ApiClientId,
                PizzeriaEnvironments.Auth.Users.Customer1.Username,
                PizzeriaEnvironments.Auth.Users.Customer1.Password,
                cancellationToken);
            this.logger.LogInformation("Step 3: Token endpoint test successful");

            this.logger.LogInformation("Auth Server configuration completed successfully");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Auth Server configuration failed: {Message}", ex.Message);
            throw;
        }
    }
}
