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

    public async Task<string> GetAdminAccessTokenAsync(CancellationToken cancellationToken)
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
            adminTokenRequest,
            cancellationToken);

        if (adminTokenResponse.IsSuccessStatusCode)
        {
            var adminToken = await adminTokenResponse.Content.ReadFromJsonAsync<KeycloakTokenResponse>(cancellationToken);
            return adminToken!.AccessToken;
        }

        var errorContent = await adminTokenResponse.Content.ReadAsStringAsync(cancellationToken);
        throw new InvalidOperationException($"Failed to authenticate with Keycloak. Status: {adminTokenResponse.StatusCode}, Error: {errorContent}");
    }

    public void SetBearerToken(string token)
    {
        this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<bool> RealmExistsAsync(string realmName, CancellationToken cancellationToken)
    {
        try
        {
            using var response = await this.httpClient.GetAsync($"/admin/realms/{realmName}", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            this.logger.LogError(ex, "Error checking realm existence");

            return false;
        }
    }

    public async Task CreateRealmAsync(string realmName, CancellationToken cancellationToken)
    {
        // Check if realm already exists
        if (await this.RealmExistsAsync(realmName, cancellationToken))
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
            registrationAllowed = false,
            loginWithEmailAllowed = true,
            duplicateEmailsAllowed = false,
            resetPasswordAllowed = true,
            editUsernameAllowed = false,
            bruteForceProtected = false,
            rememberMe = true,
            verifyEmail = false,
            accessTokenLifespan = 300,
        };

        using var realmResponse = await this.httpClient.PostAsJsonAsync(
            "/admin/realms",
            realmPayload,
            cancellationToken);
        if (!realmResponse.IsSuccessStatusCode)
        {
            var error = await realmResponse.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Failed to create realm: {error}");
        }

        this.logger.LogInformation("Realm {RealmName} created successfully", realmName);
    }

    public async Task<bool> ClientExistsAsync(
        string realmName,
        string clientId,
        CancellationToken cancellationToken)
    {
        try
        {
            using var response = await this.httpClient.GetAsync(
                $"/admin/realms/{realmName}/clients?clientId={clientId}",
                cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var clients = await response.Content.ReadFromJsonAsync<JsonElement[]>(cancellationToken);
            return clients?.Length > 0;
        }
        catch (HttpRequestException ex)
        {
            this.logger.LogError(ex, "Error checking client existence");

            return false;
        }
    }

    public async Task CreateClientAsync(
        string realmName,
        string clientId,
        string clientName,
        CancellationToken cancellationToken)
    {
        // Check if client already exists and delete it to ensure clean configuration
        if (await this.ClientExistsAsync(realmName, clientId, cancellationToken))
        {
            this.logger.LogInformation("Client {ClientId} already exists in realm {RealmName}, recreating...", clientId, realmName);
            await this.DeleteClientAsync(realmName, clientId, cancellationToken);
        }

        this.logger.LogInformation("Creating client: {ClientId} in realm: {RealmName}...", clientId, realmName);

        var clientPayload = new
        {
            clientId,
            name = clientName,
            enabled = true,
            publicClient = true,
            directAccessGrantsEnabled = true,
            standardFlowEnabled = true,
            implicitFlowEnabled = false,
            serviceAccountsEnabled = false,
            fullScopeAllowed = true,
            bearerOnly = false,
            consentRequired = false,
            protocol = "openid-connect",
            redirectUris = new[] { "*" },
            webOrigins = new[] { "*" },
            attributes = new
            {
                access_token_lifespan = "300",
            },
            protocolMappers = new object[]
            {
                new
                {
                    name = "audience-mapper",
                    protocol = "openid-connect",
                    protocolMapper = "oidc-audience-mapper",
                    consentRequired = false,
                    config = new
                    {
                        included_client_audience = clientId,
                        id_token_claim = "false",
                        access_token_claim = "true",
                    },
                },
            },
        };

        using var clientResponse = await this.httpClient.PostAsJsonAsync(
            $"/admin/realms/{realmName}/clients",
            clientPayload,
            cancellationToken);

        if (!clientResponse.IsSuccessStatusCode)
        {
            var error = await clientResponse.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Failed to create client: {error}");
        }

        this.logger.LogInformation("Client {ClientId} created successfully", clientId);
    }

    public async Task DeleteClientAsync(string realmName, string clientId, CancellationToken cancellationToken)
    {
        try
        {
            // First get the client UUID
            using var getClientResponse = await this.httpClient.GetAsync(
                $"/admin/realms/{realmName}/clients?clientId={clientId}",
                cancellationToken);

            if (!getClientResponse.IsSuccessStatusCode)
            {
                this.logger.LogWarning("Failed to find client {ClientId} for deletion", clientId);
                return;
            }

            var clients = await getClientResponse.Content.ReadFromJsonAsync<JsonElement[]>(cancellationToken);
            if (clients == null || clients.Length == 0)
            {
                this.logger.LogWarning("Client {ClientId} not found for deletion", clientId);
                return;
            }

            var clientUuid = clients[0].GetProperty("id").GetString();

            // Delete the client using the UUID
            using var deleteResponse = await this.httpClient.DeleteAsync(
                $"/admin/realms/{realmName}/clients/{clientUuid}",
                cancellationToken);

            if (deleteResponse.IsSuccessStatusCode)
            {
                this.logger.LogInformation("Client {ClientId} deleted successfully", clientId);
            }
            else
            {
                var error = await deleteResponse.Content.ReadAsStringAsync(cancellationToken);
                this.logger.LogWarning("Failed to delete client {ClientId}: {Error}", clientId, error);
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error deleting client {ClientId}", clientId);
        }
    }

    public async Task<bool> UserExistsAsync(
        string realmName,
        string username,
        CancellationToken cancellationToken)
    {
        try
        {
            using var response = await this.httpClient.GetAsync(
                $"/admin/realms/{realmName}/users?username={username}",
                cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var users = await response.Content.ReadFromJsonAsync<JsonElement[]>(cancellationToken);
            return users?.Length > 0;
        }
        catch (HttpRequestException ex)
        {
            this.logger.LogError(ex, "Error checking user existence");

            return false;
        }
    }

    public async Task CreateUserAsync(
        string realmName,
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        // Check if user already exists
        if (await this.UserExistsAsync(realmName, username, cancellationToken))
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
            email = $"{username}@example.com",
            firstName = username,
            lastName = "User",
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
            using var userResponse = await this.httpClient.PostAsJsonAsync(
                $"/admin/realms/{realmName}/users",
                userPayload,
                cancellationToken);

            if (!userResponse.IsSuccessStatusCode)
            {
                var error = await userResponse.Content.ReadAsStringAsync(cancellationToken);
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

    public async Task<string> GetUserIdAsync(
        string realmName,
        string username,
        CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Getting user ID for {Username} in realm {RealmName}...", username, realmName);

        using var response = await this.httpClient.GetAsync(
            $"/admin/realms/{realmName}/users?username={username}&exact=true",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Failed to get user {username}: {error}");
        }

        var users = await response.Content.ReadFromJsonAsync<JsonElement[]>(cancellationToken);
        if (users == null || users.Length == 0)
        {
            throw new InvalidOperationException($"User {username} not found in realm {realmName}");
        }

        var userId = users[0].GetProperty("id").GetString();
        if (string.IsNullOrEmpty(userId))
        {
            throw new InvalidOperationException($"User {username} has no ID");
        }

        this.logger.LogInformation("User {Username} has ID {UserId}", username, userId);
        return userId;
    }

    public async Task WaitForReadyAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Waiting for Keycloak to be ready...");

        var maxAttempts = 30;
        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            try
            {
                using var response = await this.httpClient.GetAsync("/realms/master", cancellationToken);
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

            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }

        throw new InvalidOperationException("Keycloak did not become ready within the timeout period");
    }

    public async Task<string> TestTokenEndpointAsync(
        string realmName,
        string clientId,
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        this.logger.LogInformation(
            "Testing token endpoint for realm {RealmName}, client {ClientId}, user {Username}",
            realmName,
            clientId,
            username);

        var tokenEndpoint = $"/realms/{realmName}/protocol/openid-connect/token";

        var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["grant_type"] = "password",
            ["client_id"] = clientId,
            ["username"] = username,
            ["password"] = password,
        });

        try
        {
            using var response = await this.httpClient.PostAsync(tokenEndpoint, tokenRequest, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(cancellationToken);
                this.logger.LogInformation("Token endpoint test successful for user {Username}", username);
                return tokenResponse!.AccessToken;
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            KeycloakErrorResponse? errorResponse = null;

            try
            {
                errorResponse = JsonSerializer.Deserialize<KeycloakErrorResponse>(errorContent);
            }
            catch
            {
                // If we can't parse as JSON, use the raw content
            }

            var errorMessage = errorResponse != null
                ? $"Error: {errorResponse.Error}, Description: {errorResponse.ErrorDescription}"
                : errorContent;

            this.logger.LogError(
                "Token endpoint test failed. Status: {StatusCode}, Error: {Error}",
                response.StatusCode,
                errorMessage);

            throw new InvalidOperationException(
                $"Token endpoint test failed for user {username}. " +
                $"Status: {response.StatusCode}, " +
                $"Error: {errorMessage}");
        }
        catch (HttpRequestException ex)
        {
            this.logger.LogError(ex, "HTTP error during token endpoint test");
            throw new InvalidOperationException("HTTP error during token endpoint test", ex);
        }
    }
}
