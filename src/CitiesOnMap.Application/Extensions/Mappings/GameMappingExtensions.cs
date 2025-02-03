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
            CityPopulation = game.GameOptions.ShowPopulation
                ? game.CurrentCity?.Population
                : null,
            Country = game.GameOptions.ShowCountry
                ? game.CurrentCity?.Country.Name
                : null,
            Points = game.Points,
            Options = game.GameOptions.ToGameOptionsModel()
        };
    }

    public static GameOptions ToGameOptions(this GameOptionsModel optionsModel)
    {
        var gameOptions = new GameOptions();
        if (optionsModel.ShowCountry.HasValue)
        {
            gameOptions.ShowCountry = optionsModel.ShowCountry.Value;
        }

        if (optionsModel.ShowPopulation.HasValue)
        {
            gameOptions.ShowPopulation = optionsModel.ShowPopulation.Value;
        }

        if (optionsModel.CitiesWithPopulationOver.HasValue)
        {
            gameOptions.CitiesWithPopulationOver = optionsModel.CitiesWithPopulationOver.Value;
        }

        if (optionsModel.CapitalsWithPopulationOver.HasValue)
        {
            gameOptions.CapitalsWithPopulationOver = optionsModel.CapitalsWithPopulationOver.Value;
        }

        if (optionsModel.DistanceUnit != null)
        {
            gameOptions.DistanceUnit = optionsModel.DistanceUnit;
        }

        if (optionsModel.MaxPointForAnswer.HasValue)
        {
            gameOptions.MaxPointForAnswer = optionsModel.MaxPointForAnswer.Value;
        }

        if (optionsModel.ReducePointsPerUnit.HasValue)
        {
            gameOptions.ReducePointsPerUnit = optionsModel.ReducePointsPerUnit.Value;
        }

        if (optionsModel.AllowNegativePoints.HasValue)
        {
            gameOptions.AllowNegativePoints = optionsModel.AllowNegativePoints.Value;
        }

        return gameOptions;
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