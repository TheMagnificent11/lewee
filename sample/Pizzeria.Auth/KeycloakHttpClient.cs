using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Pizzeria.Common;

namespace Pizzeria.Auth;

/// <summary>
/// HTTP client for Keycloak API operations
/// </summary>
public sealed class KeycloakHttpClient
{
    private readonly HttpClient httpClient;
    private readonly ILogger<KeycloakHttpClient> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeycloakHttpClient"/> class.
    /// </summary>
    /// <param name="httpClient">HTTP client</param>
    /// <param name="logger">Logger</param>
    public KeycloakHttpClient(HttpClient httpClient, ILogger<KeycloakHttpClient> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
    }

    /// <summary>
    /// Gets the admin access token
    /// </summary>
    /// <returns>Admin access token</returns>
    public async Task<string> GetAdminAccessTokenAsync()
    {
        this.logger.LogInformation("Authenticating with Keycloak admin...");

        using var adminTokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["grant_type"] = "password",
            ["client_id"] = "admin-cli",
            ["username"] = Environments.Auth.DefaultAdminCredentialsForTesting.Username,
            ["password"] = Environments.Auth.DefaultAdminCredentialsForTesting.Password,
        });

        using var adminTokenResponse = await this.httpClient.PostAsync(
            "/realms/master/protocol/openid-connect/token",
            adminTokenRequest);

        if (adminTokenResponse.IsSuccessStatusCode)
        {
            var adminToken = await adminTokenResponse.Content.ReadFromJsonAsync<KeycloakTokenResponse>();
            return adminToken!.AccessToken;
        }

        var errorContent = await adminTokenResponse.Content.ReadAsStringAsync();
        throw new InvalidOperationException($"Failed to authenticate with Keycloak. Status: {adminTokenResponse.StatusCode}, Error: {errorContent}");
    }

    /// <summary>
    /// Sets the bearer token for subsequent requests
    /// </summary>
    /// <param name="token">Bearer token</param>
    public void SetBearerToken(string token)
    {
        this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Creates a realm in Keycloak
    /// </summary>
    /// <param name="realmName">Realm name</param>
    /// <returns>Task</returns>
    public async Task CreateRealmAsync(string realmName)
    {
        this.logger.LogInformation("Creating realm: {RealmName}...", realmName);

        var realmPayload = new
        {
            realm = realmName,
            enabled = true,
            sslRequired = "none",
        };

        var realmResponse = await this.httpClient.PostAsJsonAsync("/admin/realms", realmPayload);
        if (!realmResponse.IsSuccessStatusCode)
        {
            var error = await realmResponse.Content.ReadAsStringAsync();
            if (error.Contains("Conflict", StringComparison.Ordinal))
            {
                this.logger.LogInformation("Realm {RealmName} already exists", realmName);
                return;
            }

            throw new InvalidOperationException($"Failed to create realm: {error}");
        }

        this.logger.LogInformation("Realm {RealmName} created successfully", realmName);
    }

    /// <summary>
    /// Creates a client in Keycloak
    /// </summary>
    /// <param name="realmName">Realm name</param>
    /// <param name="clientId">Client ID</param>
    /// <returns>Task</returns>
    public async Task CreateClientAsync(string realmName, string clientId)
    {
        this.logger.LogInformation("Creating client: {ClientId} in realm: {RealmName}...", clientId, realmName);

        var clientPayload = new
        {
            clientId,
            enabled = true,
            publicClient = true,
            directAccessGrantsEnabled = true,
            standardFlowEnabled = true,
            implicitFlowEnabled = false,
            redirectUris = new[] { "*" },
            webOrigins = new[] { "*" },
            attributes = new { access_token_lifespan = "300" },
        };

        using var clientResponse = await this.httpClient.PostAsJsonAsync(
            $"/admin/realms/{realmName}/clients",
            clientPayload);

        if (!clientResponse.IsSuccessStatusCode)
        {
            var error = await clientResponse.Content.ReadAsStringAsync();
            if (error.Contains("Conflict", StringComparison.Ordinal))
            {
                this.logger.LogInformation("Client {ClientId} already exists", clientId);
                return;
            }

            throw new InvalidOperationException($"Failed to create client: {error}");
        }

        this.logger.LogInformation("Client {ClientId} created successfully", clientId);
    }

    /// <summary>
    /// Creates a user in Keycloak
    /// </summary>
    /// <param name="realmName">Realm name</param>
    /// <param name="username">Username</param>
    /// <param name="password">Password</param>
    /// <returns>Task</returns>
    public async Task CreateUserAsync(string realmName, string username, string password)
    {
        this.logger.LogInformation("Creating user: {Username} in realm: {RealmName}...", username, realmName);

        var userPayload = new
        {
            username,
            enabled = true,
            credentials = new[]
            {
                new
                {
                    type = "password",
                    value = password,
                    temporary = false,
                },
            },
        };

        var userResponse = await this.httpClient.PostAsJsonAsync(
            $"/admin/realms/{realmName}/users",
            userPayload);

        if (!userResponse.IsSuccessStatusCode)
        {
            var error = await userResponse.Content.ReadAsStringAsync();
            if (error.Contains("Conflict", StringComparison.Ordinal) || error.Contains("already exists", StringComparison.Ordinal))
            {
                this.logger.LogInformation("User {Username} already exists", username);
                return;
            }

            throw new InvalidOperationException($"Failed to create user {username}: {error}");
        }

        this.logger.LogInformation("User {Username} created successfully", username);
    }

    /// <summary>
    /// Waits for Keycloak to be ready
    /// </summary>
    /// <returns>Task</returns>
    public async Task WaitForReadyAsync()
    {
        this.logger.LogInformation("Waiting for Keycloak to be ready...");

        var maxAttempts = 30;
        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            try
            {
                var response = await this.httpClient.GetAsync("/realms/master");
                if (response.IsSuccessStatusCode)
                {
                    this.logger.LogInformation("Keycloak is ready");
                    return;
                }
            }
            catch (HttpRequestException)
            {
                // Keycloak not ready yet
            }
            catch (TaskCanceledException)
            {
                // Timeout, Keycloak not ready yet
            }

            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        throw new InvalidOperationException("Keycloak did not become ready within the timeout period");
    }
}
