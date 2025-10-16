using System.Text.Json.Serialization;

namespace Pizzeria.Auth;

public sealed class KeycloakErrorResponse
{
    [JsonPropertyName("error")]
    public string Error { get; init; } = string.Empty;

    [JsonPropertyName("error_description")]
    public string ErrorDescription { get; init; } = string.Empty;
}
