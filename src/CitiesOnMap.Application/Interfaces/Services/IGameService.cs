using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Application.Features.Games.Models;

namespace CitiesOnMap.Application.Interfaces.Services;

public interface IGameService
{
    Task<OperationResult<IEnumerable<GameModel>>> GetGamesForPlayerAsync(
        string? userId, string? playerId, CancellationToken cancellationToken);

    Task<OperationResult<GameModel>> GetGameAsync(string gameId, string? userId, string? playerId,
        CancellationToken cancellationToken);

    Task<OperationResult<GameModel>> StartNewGameAsync(string? playerId,
        GameOptionsModel? optionsModel, CancellationToken cancellationToken);

    Task<OperationResult<GameModel>> GetNextQuestionAsync(string gameId, CancellationToken cancellationToken);

    Task<OperationResult<AnswerResultModel>>
        ProcessAnswerAsync(AnswerModel answer, CancellationToken cancellationToken);

    Task<OperationResult<GameModel>> UpdateGameOptionsAsync(string gameId, string? userId, string? playerId,
        GameOptionsModel optionsModel, CancellationToken cancellationToken);
}
