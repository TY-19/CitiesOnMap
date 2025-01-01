using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Commands.Games.SaveGame;

public record SaveGameCommand(Game Game) : IRequest;