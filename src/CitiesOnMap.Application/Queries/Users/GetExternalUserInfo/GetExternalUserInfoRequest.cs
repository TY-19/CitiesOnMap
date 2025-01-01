using CitiesOnMap.Application.Models.Authorization.External;
using MediatR;

namespace CitiesOnMap.Application.Queries.Users.GetExternalUserInfo;

public record GetExternalUserInfoRequest(string Provider, string Endpoint, string Token) : IRequest<ExternalUserInfo?>;