using CitiesOnMap.Application.Models.Login.External;
using MediatR;

namespace CitiesOnMap.Application.Queries.Users.GetExternalUserInfo;

public record GetExternalUserInfoRequest(string Provider, string Endpoint, string Token) : IRequest<ExternalUserInfo?>;