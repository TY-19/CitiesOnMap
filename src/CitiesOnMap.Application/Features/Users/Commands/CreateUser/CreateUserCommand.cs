using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(
    string UserName,
    string? Email,
    string? Password,
    string? Provider = null,
    string? ProviderKey = null,
    List<string>? Roles = null)
    : IRequest<OperationResult<User>>
{
}
