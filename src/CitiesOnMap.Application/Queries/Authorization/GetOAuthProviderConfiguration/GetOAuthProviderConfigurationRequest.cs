using CitiesOnMap.Application.Models.Authorization.External;
using MediatR;

namespace CitiesOnMap.Application.Queries.Authorization.GetOAuthProviderConfiguration;

public record GetOAuthProviderConfigurationRequest(string Provider) : IRequest<ExternalProviderConfiguration>;