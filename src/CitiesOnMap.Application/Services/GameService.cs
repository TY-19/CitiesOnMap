using CitiesOnMap.Application.Commands.Games.SaveGame;
using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Application.Extensions.Mappings;
using CitiesOnMap.Application.Interfaces.Services;
using CitiesOnMap.Application.Models.Game;
using CitiesOnMap.Application.Queries.Games.GetGame;
using CitiesOnMap.Application.Queries.Games.GetNextCity;
using CitiesOnMap.Domain.Constants;
using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Services;

public class GameService(IMediator mediator) : IGameService
{
    public async Task<OperationResult<GameModel>> GetGameAsync(string gameId, string playerId,
        CancellationToken cancellationToken)
    {
        Game? game = await mediator.Send(new GetGameRequest(gameId), cancellationToken);
        if (game == null)
        {
            return new OperationResult<GameModel>(false, ResultType.GameNotExist);
        }

        return game.PlayerId == playerId
            ? new OperationResult<GameModel>(true, game.ToGameModel())
            : new OperationResult<GameModel>(false, ResultType.InvalidPlayerForGame);
    }

    public async Task<OperationResult<GameModel>> StartNewGameAsync(
        string? playerId, GameOptionsModel? optionsModel, CancellationToken cancellationToken)
    {
        playerId ??= Guid.NewGuid().ToString();
        GameOptions options = optionsModel == null
            ? new GameOptions()
            : optionsModel.ToGameOptions();
        var game = new Game { PlayerId = playerId, GameOptions = options };
        await mediator.Send(new SaveGameCommand(game), cancellationToken);
        return new OperationResult<GameModel>(true, game.ToGameModel());
    }

    public async Task<OperationResult<GameModel>> GetNextQuestionAsync(
        string gameId, CancellationToken cancellationToken)
    {
        Game? game = await mediator.Send(new GetGameRequest(gameId), cancellationToken);
        if (game == null)
        {
            return new OperationResult<GameModel>(false, ResultType.GameNotExist);
        }

        var request = new GetNextCityRequest(game);
        City? city = await mediator.Send(request, cancellationToken);
        game.CurrentCity = city;
        await mediator.Send(new SaveGameCommand(game), cancellationToken);
        return new OperationResult<GameModel>(true, game.ToGameModel());
    }

    public async Task<OperationResult<AnswerResultModel>> ProcessAnswerAsync(
        AnswerModel answer, CancellationToken cancellationToken)
    {
        Game? game = await mediator.Send(new GetGameRequest(answer.GameId), cancellationToken);
        if (game?.CurrentCity == null)
        {
            return new OperationResult<AnswerResultModel>(false, ResultType.NoCityInQuestion);
        }

        double distance = CalculateDistance(game.CurrentCity, answer);
        if (game.GameOptions.DistanceUnit == "mi")
        {
            distance *= AppConstants.KilometersInMile;
        }

        int points = game.GameOptions.MaxPointForAnswer - (int)distance * game.GameOptions.ReducePointsPerUnit;
        if (points < 0 && !game.GameOptions.AllowNegativePoints)
        {
            points = 0;
        }

        var answerResult = new AnswerResultModel
        {
            Answer = answer,
            City = game.CurrentCity.ToCityDto(),
            Distance = distance,
            Points = points
        };
        game.Previous.Add(game.CurrentCity.Id);
        game.CurrentCity = null;
        game.Points += points;
        game.LastPlayTime = DateTimeOffset.UtcNow;
        await mediator.Send(new SaveGameCommand(game), cancellationToken);
        return new OperationResult<AnswerResultModel>(true, answerResult);
    }

    private static double CalculateDistance(City city, AnswerModel answer)
    {
        double f1 = (double)city.Latitude * Math.PI / 180;
        double l1 = (double)city.Longitude * Math.PI / 180;
        double f2 = (double)answer.SelectedLatitude * Math.PI / 180;
        double l2 = (double)answer.SelectedLongitude * Math.PI / 180;
        double deltaL = l1 * l2 >= 0 ? Math.Abs(l1 - l2) : Math.Abs(l1) + Math.Abs(l2);
        if (deltaL > Math.PI)
        {
            deltaL = Math.PI - deltaL % Math.PI;
        }

        double distance = Math.Atan2(Math.Sqrt(Math.Pow(Math.Cos(f2) * Math.Sin(deltaL), 2)
                                               + Math.Pow(
                                                   Math.Cos(f1) * Math.Sin(f2) -
                                                   Math.Sin(f1) * Math.Cos(f2) * Math.Cos(deltaL), 2)),
            Math.Sin(f1) * Math.Sin(f2) + Math.Cos(f1) * Math.Cos(f2) * Math.Cos(deltaL));
        return distance * AppConstants.EarthRadiusAvg;
    }
}