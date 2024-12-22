using CitiesOnMap.Application.Models;
using CitiesOnMap.Domain.Entities;

namespace CitiesOnMap.Application.Extensions;

public static class MappingExtensions
{
    public static GameModel ToGameModel(this Game game)
    {
        return new GameModel
        {
            Id = game.Id,
            PlayerId = game.PlayerId,
            CurrentCityName = game.CurrentCity?.Name,
            Points = game.Points
        };
    }
}