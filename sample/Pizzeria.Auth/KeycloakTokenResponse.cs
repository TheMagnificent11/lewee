using System.Text.Json.Serialization;

namespace Pizzeria.Auth;

/// <summary>
/// Keycloak token response
/// </summary>
public sealed class KeycloakTokenResponse
{
    /// <summary>
    /// Gets or sets the access token
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expires in seconds
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets the refresh expires in seconds
    /// </summary>
    [JsonPropertyName("refresh_expires_in")]
    public int RefreshExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets the token type
    /// </summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;
}
