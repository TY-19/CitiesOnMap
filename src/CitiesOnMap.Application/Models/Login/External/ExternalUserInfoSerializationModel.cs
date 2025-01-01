using System.Text.Json.Serialization;

namespace CitiesOnMap.Application.Models.Login.External;

public class ExternalUserInfoSerializationModel
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }
}