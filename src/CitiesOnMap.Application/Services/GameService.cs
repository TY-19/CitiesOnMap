using CitiesOnMap.Application.Commands.SaveGame;
using CitiesOnMap.Application.Common;
using CitiesOnMap.Application.Extensions;
using CitiesOnMap.Application.Interfaces;
using CitiesOnMap.Application.Models;
using CitiesOnMap.Application.Queries.GetGame;
using CitiesOnMap.Application.Queries.GetNextCity;
using CitiesOnMap.Domain.Constants;
using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Services;

public class GameService(IMediator mediator) : IGameService
{
    public async Task<OperationResult<GameModel>> StartNewGameAsync(
        string? playerId, CancellationToken cancellationToken)
    {
        playerId ??= Guid.NewGuid().ToString();
        var game = new Game { PlayerId = playerId };
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

        var request = new GetNextCityRequest
        {
            Previous = game.Previous
        };
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
        int points = 5000 - (int)distance;
        var answerResult = new AnswerResultModel
        {
            Answer = answer,
            City = game.CurrentCity,
            Distance = distance,
            Points = points
        };
        game.Previous.Add(game.CurrentCity.Name);
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