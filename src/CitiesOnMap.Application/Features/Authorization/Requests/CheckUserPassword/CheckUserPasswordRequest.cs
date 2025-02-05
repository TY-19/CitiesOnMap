using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Features.Authorization.Requests.CheckUserPassword;

public record CheckUserPasswordRequest(User User, string Password) : IRequest<OperationResult>;
