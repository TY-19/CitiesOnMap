using System.Text.Json.Serialization;

namespace CitiesOnMap.Application.Models;

public class OAuthRequest
{
    [JsonPropertyName("client_id")] public string ClientId { get; set; } = null!;
}