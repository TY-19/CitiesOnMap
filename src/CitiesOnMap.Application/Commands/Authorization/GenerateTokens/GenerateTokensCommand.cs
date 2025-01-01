using CitiesOnMap.Application.Models.Authorization;
using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Commands.Authorization.GenerateTokens;

public record GenerateTokensCommand(User User) : IRequest<TokensModel>;