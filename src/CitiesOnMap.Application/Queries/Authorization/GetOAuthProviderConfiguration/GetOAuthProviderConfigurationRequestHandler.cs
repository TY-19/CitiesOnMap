using CitiesOnMap.Application.Interfaces.Helpers;
using CitiesOnMap.Application.Models.Authorization.External;
using MediatR;

namespace CitiesOnMap.Application.Queries.Authorization.GetOAuthProviderConfiguration;

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