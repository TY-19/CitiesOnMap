using CitiesOnMap.Application.Models.Authorization.External;
using MediatR;

namespace CitiesOnMap.Application.Queries.Users.GetOAuthProviderConfiguration;

public record GetOAuthProviderConfigurationRequest(string Provider) : IRequest<ExternalProviderConfiguration>;