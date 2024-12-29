using System.Text.Json.Serialization;

namespace CitiesOnMap.Application.Models;

public class ExternalUserInfo
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }
}