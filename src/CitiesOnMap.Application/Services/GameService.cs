using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Application.Features.Games.Commands.CheckAnswer;
using CitiesOnMap.Application.Features.Games.Commands.PatchGameOptions;
using CitiesOnMap.Application.Features.Games.Commands.SaveGame;
using CitiesOnMap.Application.Features.Games.Extensions;
using CitiesOnMap.Application.Features.Games.Models;
using CitiesOnMap.Application.Features.Games.Requests.GetGame;
using CitiesOnMap.Application.Features.Games.Requests.GetGamesForPlayer;
using CitiesOnMap.Application.Features.Games.Requests.GetNextCity;
using CitiesOnMap.Application.Features.Users.Requests.GetUser;
using CitiesOnMap.Application.Interfaces.Services;
using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Services;

public class GameService(IMediator mediator) : IGameService
{
    public async Task<OperationResult<IEnumerable<GameModel>>> GetGamesForPlayerAsync(
        string? userId, string? playerId, CancellationToken cancellationToken)
    {
        List<GameModel> games = [];
        if (userId != null)
        {
            games.AddRange((await mediator.Send(new GetGamesForPlayerRequest(userId), cancellationToken))
                .Select(g => g.ToGameModel()));
        }

        if (playerId == null || playerId == userId)
        {
            return new OperationResult<IEnumerable<GameModel>>(true, games);
        }

        User? user = await mediator.Send(new GetUserRequest(
            playerId, null, null, null, null), cancellationToken);
        if (user == null)
        {
            games.AddRange((await mediator.Send(new GetGamesForPlayerRequest(playerId), cancellationToken))
                .Select(g => g.ToGameModel()));
        }

        return new OperationResult<IEnumerable<GameModel>>(true, games);
    }

    public async Task<OperationResult<GameModel>> GetGameAsync(
        string gameId, string? userId, string? playerId, CancellationToken cancellationToken)
    {
        Game? game = await mediator.Send(new GetGameRequest(gameId), cancellationToken);
        if (game == null)
        {
            return new OperationResult<GameModel>(false, ResultType.GameNotExist);
        }

        if (!await IsGameOwnedByPlayer(userId, playerId, game, cancellationToken))
        {
            return new OperationResult<GameModel>(false, ResultType.InvalidPlayerForGame);
        }

        return new OperationResult<GameModel>(true, game.ToGameModel());
    }

    public async Task<OperationResult<GameModel>> StartNewGameAsync(
        string? playerId, GameOptionsModel? optionsModel, CancellationToken cancellationToken)
    {
        playerId ??= Guid.NewGuid().ToString();
        var options = new GameOptions();
        if (optionsModel != null)
        {
            await mediator.Send(new PatchGameOptionsCommand(options, optionsModel), cancellationToken);
        }

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

        AnswerResultModel answerResult = await mediator.Send(new CheckAnswerCommand(game, answer), cancellationToken);

        await mediator.Send(new SaveGameCommand(game), cancellationToken);
        return new OperationResult<AnswerResultModel>(true, answerResult);
    }

    public async Task<OperationResult<GameModel>> UpdateGameOptionsAsync(string gameId,
        string? userId, string? playerId, GameOptionsModel optionsModel, CancellationToken cancellationToken)
    {
        Game? game = await mediator.Send(new GetGameRequest(gameId), cancellationToken);
        if (game == null)
        {
            return new OperationResult<GameModel>(false, ResultType.GameNotExist);
        }

        if (!await IsGameOwnedByPlayer(userId, playerId, game, cancellationToken))
        {
            return new OperationResult<GameModel>(false, ResultType.InvalidPlayerForGame);
        }

        await mediator.Send(new PatchGameOptionsCommand(game.GameOptions, optionsModel), cancellationToken);
        await mediator.Send(new SaveGameCommand(game), cancellationToken);

        return new OperationResult<GameModel>(true, game.ToGameModel());
    }

    private async Task<bool> IsGameOwnedByPlayer(string? userId, string? playerId, Game game,
        CancellationToken cancellationToken)
    {
        User? user = await mediator.Send(new GetUserRequest(
            game.PlayerId, null, null, null, null), cancellationToken);
        return user == null
            ? game.PlayerId == playerId
            : user.Id == userId && game.PlayerId == userId;
    }
}
