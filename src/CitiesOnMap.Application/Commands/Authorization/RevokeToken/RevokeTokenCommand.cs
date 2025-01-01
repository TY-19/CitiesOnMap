using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Commands.Authorization.RevokeToken;

public record RevokeTokenCommand(User User, string token, string? tokenType) : IRequest<OperationResult>;