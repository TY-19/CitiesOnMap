using CitiesOnMap.Application.Models.Login;
using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Commands.Users.GenerateTokens;

public record GenerateTokensCommand(User User) : IRequest<TokensModel>;