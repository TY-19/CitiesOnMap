using System.Text.Json.Serialization;

namespace CitiesOnMap.Application.Models.Authorization.External;

public class ExternalUserInfoSerializationModel
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }
}