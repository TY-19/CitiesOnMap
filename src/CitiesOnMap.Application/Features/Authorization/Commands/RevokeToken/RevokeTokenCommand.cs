using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Features.Authorization.Commands.RevokeToken;

public record RevokeTokenCommand(User User, string token, string? tokenType) : IRequest<OperationResult>;
