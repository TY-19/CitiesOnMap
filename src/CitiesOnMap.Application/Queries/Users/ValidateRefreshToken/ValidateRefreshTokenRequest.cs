using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Queries.Users.ValidateRefreshToken;

public record ValidateRefreshTokenRequest(User User, string RefreshToken) : IRequest<OperationResult>;