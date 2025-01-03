using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Queries.Authorization.CheckUserPassword;

public record CheckUserPasswordRequest(User User, string Password) : IRequest<OperationResult>;