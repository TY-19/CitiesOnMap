using CitiesOnMap.Application.Models.Game;
using CitiesOnMap.Domain.Entities;

namespace CitiesOnMap.Application.Extensions.Mappings;

public static class GameMappingExtensions
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