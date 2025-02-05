using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Features.Games.Commands.SaveGame;

public record SaveGameCommand(Game Game) : IRequest;
