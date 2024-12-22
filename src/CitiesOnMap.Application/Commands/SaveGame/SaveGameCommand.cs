using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Commands.SaveGame;

public class SaveGameCommand(Game game) : IRequest
{
    public Game Game { get; } = game;
}