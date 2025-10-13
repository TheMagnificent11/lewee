using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Pizzeria.Common;

namespace Pizzeria.Auth;

public sealed class KeycloakHttpClient
{
    private readonly HttpClient httpClient;
    private readonly ILogger<KeycloakHttpClient> logger;

    public KeycloakHttpClient(HttpClient httpClient, ILogger<KeycloakHttpClient> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
    }

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

    public void SetBearerToken(string token)
    {
        this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<bool> RealmExistsAsync(string realmName)
    {
        try
        {
            var response = await this.httpClient.GetAsync($"/admin/realms/{realmName}");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            this.logger.LogError(ex, "Error checking realm existence");

            return false;
        }
    }

    public async Task CreateRealmAsync(string realmName)
    {
        // Check if realm already exists
        if (await this.RealmExistsAsync(realmName))
        {
            this.logger.LogInformation("Realm {RealmName} already exists", realmName);
            return;
        }

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
            throw new InvalidOperationException($"Failed to create realm: {error}");
        }

        this.logger.LogInformation("Realm {RealmName} created successfully", realmName);
    }

    public async Task<bool> ClientExistsAsync(string realmName, string clientId)
    {
        try
        {
            var response = await this.httpClient.GetAsync($"/admin/realms/{realmName}/clients?clientId={clientId}");
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var clients = await response.Content.ReadFromJsonAsync<JsonElement[]>();
            return clients?.Length > 0;
        }
        catch (HttpRequestException ex)
        {
            this.logger.LogError(ex, "Error checking client existence");

            return false;
        }
    }

    public async Task CreateClientAsync(string realmName, string clientId, string clientName)
    {
        // Check if client already exists
        if (await this.ClientExistsAsync(realmName, clientId))
        {
            this.logger.LogInformation("Client {ClientId} already exists in realm {RealmName}", clientId, realmName);
            return;
        }

        this.logger.LogInformation("Creating client: {ClientId} in realm: {RealmName}...", clientId, realmName);

        var clientPayload = new
        {
            clientId,
            enabled = true,
            publicClient = true,
            directAccessGrantsEnabled = true,
            serviceAccountsEnabled = false,
            standardFlowEnabled = true,
            implicitFlowEnabled = false,
            fullScopeAllowed = true,
            redirectUris = new[] { "*" },
            webOrigins = new[] { "*" },
            attributes = new
            {
                access_token_lifespan = "300",
                client_credentials_use_refresh_token = "false",
            },
            protocolMappers = new[]
            {
                new
                {
                    name = clientName,
                    protocol = "openid-connect",
                    protocolMapper = "oidc-usersessionmodel-note-mapper",
                    consentRequired = false,
                    config = new
                    {
                        user_session_note = "clientId",
                        id_token_claim = "true",
                        access_token_claim = "true",
                        claim_name = "clientId",
                        jsonType_label = "String",
                    },
                },
            },
        };

        using var clientResponse = await this.httpClient.PostAsJsonAsync(
            $"/admin/realms/{realmName}/clients",
            clientPayload);

        if (!clientResponse.IsSuccessStatusCode)
        {
            var error = await clientResponse.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Failed to create client: {error}");
        }

        this.logger.LogInformation("Client {ClientId} created successfully", clientId);
    }

    public async Task<bool> UserExistsAsync(string realmName, string username)
    {
        try
        {
            var response = await this.httpClient.GetAsync($"/admin/realms/{realmName}/users?username={username}");
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var users = await response.Content.ReadFromJsonAsync<JsonElement[]>();
            return users?.Length > 0;
        }
        catch (HttpRequestException ex)
        {
            this.logger.LogError(ex, "Error checking user existence");

            return false;
        }
    }

    public async Task CreateUserAsync(string realmName, string username, string password)
    {
        // Check if user already exists
        if (await this.UserExistsAsync(realmName, username))
        {
            this.logger.LogInformation("User {Username} already exists in realm {RealmName}", username, realmName);
            return;
        }

        this.logger.LogInformation("Creating user: {Username} in realm: {RealmName}...", username, realmName);

        var userPayload = new
        {
            username,
            enabled = true,
            emailVerified = true,
            requiredActions = Array.Empty<string>(),
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

        try
        {
            var userResponse = await this.httpClient.PostAsJsonAsync(
                $"/admin/realms/{realmName}/users",
                userPayload);

            if (!userResponse.IsSuccessStatusCode)
            {
                var error = await userResponse.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Failed to create user {username}: {error}");
            }

            this.logger.LogInformation("User {Username} created successfully", username);
        }
        catch (HttpRequestException ex)
        {
            this.logger.LogError(ex, "Error creating user");

            throw;
        }
    }

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
