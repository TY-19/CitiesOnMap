using CitiesOnMap.Application.Features.Games.Models;
using CitiesOnMap.Domain.Entities;

namespace CitiesOnMap.Application.Features.Games.Extensions;

public static class GameMappingExtensions
{
    public static GameModel ToGameModel(this Game game)
    {
        return new GameModel
        {
            Id = game.Id,
            PlayerId = game.PlayerId,
            CurrentCityName = game.CurrentCity?.Name,
            CityPopulation = game.GameOptions.ShowPopulation
                ? game.CurrentCity?.Population
                : null,
            Country = game.GameOptions.ShowCountry
                ? game.CurrentCity?.Country.Name
                : null,
            Points = game.Points,
            LastPlayTime = game.LastPlayTime,
            Options = game.GameOptions.ToGameOptionsModel()
        };
    }

    public static GameOptionsModel ToGameOptionsModel(this GameOptions options)
    {
        return new GameOptionsModel
        {
            ShowCountry = options.ShowCountry,
            ShowPopulation = options.ShowPopulation,
            CapitalsWithPopulationOver = options.CapitalsWithPopulationOver,
            CitiesWithPopulationOver = options.CitiesWithPopulationOver,
            DistanceUnit = options.DistanceUnit,
            MaxPointForAnswer = options.MaxPointForAnswer,
            ReducePointsPerUnit = options.ReducePointsPerUnit,
            AllowNegativePoints = options.AllowNegativePoints
        };
    }

    public static CityDto ToCityDto(this City city)
    {
        return new CityDto
        {
            Id = city.Id,
            Name = city.Name,
            NameAscii = city.NameAscii,
            Latitude = city.Latitude,
            Longitude = city.Longitude,
            CountryId = city.CountryId,
            Country = city.Country.Name,
            AdministrationName = city.AdministrationName,
            CapitalType = city.CapitalType,
            Population = city.Population
        };
    }
}
