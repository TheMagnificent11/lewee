using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Pizzeria.Auth;

internal sealed class KeycloakHttpClient : IAuthServerAdminClient
{
    private readonly HttpClient httpClient;
    private readonly ILogger<KeycloakHttpClient> logger;

    public KeycloakHttpClient(HttpClient httpClient, ILogger<KeycloakHttpClient> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
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
