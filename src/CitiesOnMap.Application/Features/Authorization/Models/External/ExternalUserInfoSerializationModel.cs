using System.Text.Json.Serialization;

namespace CitiesOnMap.Application.Features.Authorization.Models.External;

public class ExternalUserInfoSerializationModel
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }
}
