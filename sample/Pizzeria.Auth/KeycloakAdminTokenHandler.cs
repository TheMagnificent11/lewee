using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Pizzeria.Common;

namespace Pizzeria.Auth;

[SuppressMessage(
    "Performance",
    "CA1812: Avoid uninstantiated internal classes",
    Justification = "Use via DI")]
internal sealed class KeycloakAdminTokenHandler : DelegatingHandler
{
    private const string TokenCacheKey = "KeycloakAdminToken";
    private readonly IMemoryCache memoryCache;
    private readonly ILogger<KeycloakAdminTokenHandler> logger;
    private readonly SemaphoreSlim semaphore = new(1, 1);

    public KeycloakAdminTokenHandler(IMemoryCache memoryCache, ILogger<KeycloakAdminTokenHandler> logger)
    {
        this.memoryCache = memoryCache;
        this.logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Get or create admin token
        var token = await this.GetOrCreateAdminTokenAsync(request.RequestUri!, cancellationToken);

        // Add authorization header
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.semaphore.Dispose();
        }

        base.Dispose(disposing);
    }

    private async Task<string> GetOrCreateAdminTokenAsync(Uri requestUri, CancellationToken cancellationToken)
    {
        // Try to get from cache first
        if (this.memoryCache.TryGetValue(TokenCacheKey, out string? cachedToken) && !string.IsNullOrEmpty(cachedToken))
        {
            return cachedToken;
        }

        // Use semaphore to ensure only one token request at a time
        await this.semaphore.WaitAsync(cancellationToken);
        try
        {
            // Double-check after acquiring semaphore
            if (this.memoryCache.TryGetValue(TokenCacheKey, out cachedToken) && !string.IsNullOrEmpty(cachedToken))
            {
                return cachedToken;
            }

            // Get new token
            this.logger.LogInformation("Authenticating with Keycloak admin...");

            using var tokenRequest = new FormUrlEncodedContent(new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["grant_type"] = "password",
                ["client_id"] = "admin-cli",
                ["username"] = Environments.Auth.DefaultAdminCredentialsForTesting.Username,
                ["password"] = Environments.Auth.DefaultAdminCredentialsForTesting.Password,
            });

            // Use the base URI from the request to get token
            var baseUri = requestUri.GetLeftPart(UriPartial.Authority);
            using var tokenHttpClient = new HttpClient { BaseAddress = new Uri(baseUri) };

            using var tokenResponse = await tokenHttpClient.PostAsync(
                "/realms/master/protocol/openid-connect/token",
                tokenRequest,
                cancellationToken);

            if (!tokenResponse.IsSuccessStatusCode)
            {
                var errorContent = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"Failed to authenticate with Keycloak. Status: {tokenResponse.StatusCode}, Error: {errorContent}");
            }

            var tokenData = await tokenResponse.Content.ReadFromJsonAsync<KeycloakTokenResponse>(cancellationToken);
            if (tokenData == null || string.IsNullOrEmpty(tokenData.AccessToken))
            {
                throw new InvalidOperationException("Failed to parse Keycloak admin token response");
            }

            // Cache the token until just before expiry (subtract 30 seconds for safety margin)
            var expiryTime = TimeSpan.FromSeconds(Math.Max(tokenData.ExpiresIn - 30, 60));
            this.memoryCache.Set(TokenCacheKey, tokenData.AccessToken, expiryTime);

            this.logger.LogInformation("Admin token cached for {ExpirySeconds} seconds", expiryTime.TotalSeconds);

            return tokenData.AccessToken;
        }
        finally
        {
            this.semaphore.Release();
        }
    }
}
