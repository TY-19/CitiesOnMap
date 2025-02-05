using CitiesOnMap.Application.Features.Authorization.Models.External;
using MediatR;

namespace CitiesOnMap.Application.Features.Authorization.Requests.GetExternalUserInfo;

public record GetExternalUserInfoRequest(string Provider, string Endpoint, string Token) : IRequest<ExternalUserInfo?>;
