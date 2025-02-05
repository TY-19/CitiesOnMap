using CitiesOnMap.Application.Features.Authorization.Models.External;
using MediatR;

namespace CitiesOnMap.Application.Features.Authorization.Requests.GetOAuthProviderConfiguration;

public record GetOAuthProviderConfigurationRequest(string Provider) : IRequest<ExternalProviderConfiguration>;
