using CitiesOnMap.Application.Features.Authorization.Models.External;
using CitiesOnMap.Application.Interfaces.Helpers;
using MediatR;

namespace CitiesOnMap.Application.Features.Authorization.Requests.GetOAuthProviderConfiguration;

public class GetOAuthProviderConfigurationRequestHandler(IConfigurationHelper configurationHelper)
    : IRequestHandler<GetOAuthProviderConfigurationRequest, ExternalProviderConfiguration>
{
    public Task<ExternalProviderConfiguration> Handle(GetOAuthProviderConfigurationRequest request,
        CancellationToken cancellationToken)
    {
        string section = $"OAuth:{request.Provider}:";
        ExternalProviderConfiguration config = new()
        {
            ClientId = configurationHelper.GetConfigurationValue("OAuth:Google:ClientId") ?? "",
            ClientSecret = configurationHelper.GetConfigurationValue("OAuth:Google:ClientSecret") ?? "",
            TokenEndpoint = configurationHelper.GetConfigurationValue($"{section}:TokenEndpoint") ?? "",
            UserInfoEndpoint = configurationHelper.GetConfigurationValue("OAuth:Google:UserInfoEndpoint") ?? "",
            FrontendCallbackUrl = configurationHelper.GetConfigurationValue("OAuth:Google:FrontendCallbackUrl") ?? ""
        };
        return Task.FromResult(config);
    }
}
