using CitiesOnMap.Application.Models.Authorization.External;
using MediatR;

namespace CitiesOnMap.Application.Queries.Authorization.GetExternalUserInfo;

public record GetExternalUserInfoRequest(string Provider, string Endpoint, string Token) : IRequest<ExternalUserInfo?>;