using CitiesOnMap.Application.Features.Authorization.Models;
using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Features.Authorization.Commands.GenerateTokens;

public record GenerateTokensCommand(User User) : IRequest<TokensModel>;
