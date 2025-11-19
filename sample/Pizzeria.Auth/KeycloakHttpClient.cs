using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pizzeria.Auth;

internal sealed class KeycloakHttpClient : IAuthServerAdminClient
{
    private readonly HttpClient httpClient;
    private readonly string realmName;
    private readonly ILogger<KeycloakHttpClient> logger;

    public KeycloakHttpClient(
        [NotNull] IOptions<KeycloakOptions> options,
        HttpClient httpClient,
        ILogger<KeycloakHttpClient> logger)
    {
        this.realmName = options.Value.RealmName;

        this.httpClient = httpClient;
        this.logger = logger;
    }

    public async Task<bool> UserExistsAsync(
        string username,
        CancellationToken cancellationToken)
    {
        try
        {
            using var response = await this.httpClient.GetAsync(
                $"/admin/realms/{this.realmName}/users?username={username}",
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
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        // Check if user already exists
        if (await this.UserExistsAsync(username, cancellationToken))
        {
            this.logger.LogInformation("User {Username} already exists", username);
            return;
        }

        this.logger.LogInformation("Creating user: {Username}", username);

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
                $"/admin/realms/{this.realmName}/users",
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
        string username,
        CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Getting user ID for {Username}", username);

        using var response = await this.httpClient.GetAsync(
            $"/admin/realms/{this.realmName}/users?username={username}&exact=true",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Failed to get user {username}: {error}");
        }

        var users = await response.Content.ReadFromJsonAsync<JsonElement[]>(cancellationToken);
        if (users == null || users.Length == 0)
        {
            throw new InvalidOperationException($"User {username} not found");
        }

        var userId = users[0].GetProperty("id").GetString();
        if (string.IsNullOrEmpty(userId))
        {
            throw new InvalidOperationException($"User {username} has no ID");
        }

        this.logger.LogInformation("User {Username} has ID {UserId}", username, userId);
        return userId;
    }
}
