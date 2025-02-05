using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Features.Users.Requests.GetUser;

public record GetUserRequest(
    string? UserId,
    string? UserName,
    string? Email,
    string? Provider,
    string? ProviderKey) : IRequest<User?>;
