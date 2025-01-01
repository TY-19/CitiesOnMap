namespace CitiesOnMap.Application.Models.Authorization.External;

public class ExternalProviderConfiguration
{
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
    public string TokenEndpoint { get; set; } = null!;
    public string UserInfoEndpoint { get; set; } = null!;
    public string FrontendCallbackUrl { get; set; } = null!;
}