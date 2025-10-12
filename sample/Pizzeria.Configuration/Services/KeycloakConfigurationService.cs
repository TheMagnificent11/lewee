using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pizzeria.Common;

namespace Pizzeria.Configuration.Services;

public sealed class KeycloakConfigurationService : IKeycloakConfigurationService
{
    private const string CustomerUsername = "customer";
    private const string FrontStaffUsername = "front-staff";
    private const string KitchenStaffUsername = "kitchen-staff";
    private const string DeliveryDriverUsername = "delivery-driver";
    private const string DefaultPassword = "Password123!";

    private readonly IHttpClientFactory httpClientFactory;
    private readonly IConfiguration configuration;
    private readonly ILogger<KeycloakConfigurationService> logger;

    public KeycloakConfigurationService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<KeycloakConfigurationService> logger)
    {
        this.httpClientFactory = httpClientFactory;
        this.configuration = configuration;
        this.logger = logger;
    }

    public async Task ConfigureAsync()
    {
        this.logger.LogInformation("Starting Keycloak configuration...");

        var keycloakBaseUrl = await this.GetKeycloakBaseUrlAsync();

        this.logger.LogInformation("Keycloak base URL: {BaseUrl}", keycloakBaseUrl);

        await this.WaitForKeycloakReadyAsync(keycloakBaseUrl);

        using var httpClient = this.httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(keycloakBaseUrl);

        var adminAccessToken = await this.GetAdminAccessTokenAsync(httpClient);

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminAccessToken);

        await this.CreatePizzeriaRealmAsync(httpClient);
        await this.CreateStoreApiClientAsync(httpClient);
        await this.CreateTestUserAsync(httpClient, CustomerUsername, DefaultPassword);
        await this.CreateTestUserAsync(httpClient, FrontStaffUsername, DefaultPassword);
        await this.CreateTestUserAsync(httpClient, KitchenStaffUsername, DefaultPassword);
        await this.CreateTestUserAsync(httpClient, DeliveryDriverUsername, DefaultPassword);

        this.logger.LogInformation("Keycloak configuration completed successfully");
    }

    private Task<string> GetKeycloakBaseUrlAsync()
    {
        var connectionString = this.configuration.GetConnectionString(ServiceNames.AuthServer);
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Connection string for '{ServiceNames.AuthServer}' not found");
        }

        // Connection string format: "Endpoint=http://localhost:8080"
        if (connectionString.StartsWith("Endpoint=", StringComparison.OrdinalIgnoreCase))
        {
            var baseUrl = connectionString["Endpoint=".Length..];
            return Task.FromResult(baseUrl.TrimEnd('/'));
        }

        // Fallback if it's just a URL
        return Task.FromResult(connectionString.TrimEnd('/'));
    }

    private async Task WaitForKeycloakReadyAsync(string keycloakBaseUrl)
    {
        this.logger.LogInformation("Waiting for Keycloak to be ready...");

        using var httpClient = this.httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(keycloakBaseUrl);
        httpClient.Timeout = TimeSpan.FromSeconds(5);

        var maxAttempts = 30;
        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            try
            {
                var response = await httpClient.GetAsync("/realms/master");
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

    private async Task<string> GetAdminAccessTokenAsync(HttpClient httpClient)
    {
        this.logger.LogInformation("Authenticating with Keycloak admin...");

        using var adminTokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["grant_type"] = "password",
            ["client_id"] = "admin-cli",
            ["username"] = Environments.Auth.DefaultAdminCredentialsForTesting.Username,
            ["password"] = Environments.Auth.DefaultAdminCredentialsForTesting.Password,
        });

        using var adminTokenResponse = await httpClient.PostAsync(
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

    private async Task CreatePizzeriaRealmAsync(HttpClient httpClient)
    {
        this.logger.LogInformation("Creating Pizzeria realm...");

        var realmPayload = new
        {
            realm = Environments.Auth.RealmName,
            enabled = true,
            sslRequired = "none",
        };

        var realmResponse = await httpClient.PostAsJsonAsync("/admin/realms", realmPayload);
        if (!realmResponse.IsSuccessStatusCode)
        {
            var error = await realmResponse.Content.ReadAsStringAsync();
            if (error.Contains("Conflict", StringComparison.Ordinal))
            {
                this.logger.LogInformation("Pizzeria realm already exists");
                return;
            }

            throw new InvalidOperationException($"Failed to create realm: {error}");
        }

        this.logger.LogInformation("Pizzeria realm created successfully");
    }

    private async Task CreateStoreApiClientAsync(HttpClient httpClient)
    {
        this.logger.LogInformation("Creating Store API client...");

        var clientPayload = new
        {
            clientId = Environments.Auth.ApiClientId,
            enabled = true,
            publicClient = true,
            directAccessGrantsEnabled = true,
            standardFlowEnabled = true,
            implicitFlowEnabled = false,
            redirectUris = new[] { "*" },
            webOrigins = new[] { "*" },
            attributes = new { access_token_lifespan = "300" },
        };

        using var clientResponse = await httpClient.PostAsJsonAsync(
            $"/admin/realms/{Environments.Auth.RealmName}/clients",
            clientPayload);

        if (!clientResponse.IsSuccessStatusCode)
        {
            var error = await clientResponse.Content.ReadAsStringAsync();
            if (error.Contains("Conflict", StringComparison.Ordinal))
            {
                this.logger.LogInformation("Store API client already exists");
                return;
            }

            throw new InvalidOperationException($"Failed to create client: {error}");
        }

        this.logger.LogInformation("Store API client created successfully");
    }

    private async Task CreateTestUserAsync(HttpClient httpClient, string username, string password)
    {
        this.logger.LogInformation("Creating test user: {Username}...", username);

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

        var userResponse = await httpClient.PostAsJsonAsync(
            $"/admin/realms/{Environments.Auth.RealmName}/users",
            userPayload);

        if (!userResponse.IsSuccessStatusCode)
        {
            var error = await userResponse.Content.ReadAsStringAsync();
            if (error.Contains("Conflict", StringComparison.Ordinal) || error.Contains("already exists", StringComparison.Ordinal))
            {
                this.logger.LogInformation("Test user {Username} already exists", username);
                return;
            }

            throw new InvalidOperationException($"Failed to create user {username}: {error}");
        }

        this.logger.LogInformation("Test user {Username} created successfully", username);
    }

    private sealed class KeycloakTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_expires_in")]
        public int RefreshExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = string.Empty;
    }
}
