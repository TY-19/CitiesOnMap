namespace CitiesOnMap.Application.Features.Authorization.Models.External;

public class ExternalUserInfo(string userName, string email, string provider, string providerKey)
{
    public string UserName { get; set; } = userName;
    public string Email { get; set; } = email;
    public string Provider { get; set; } = provider;
    public string ProviderKey { get; set; } = providerKey;
}
