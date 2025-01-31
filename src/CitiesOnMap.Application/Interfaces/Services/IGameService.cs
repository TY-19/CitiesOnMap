using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Application.Models.Game;

namespace CitiesOnMap.Application.Interfaces.Services;

public interface IGameService
{
    Task<OperationResult<GameModel>> GetGameAsync(string gameId, string playerId,
        CancellationToken cancellationToken);

    Task<OperationResult<GameModel>> StartNewGameAsync(string? playerId, CancellationToken cancellationToken);
    Task<OperationResult<GameModel>> GetNextQuestionAsync(string gameId, CancellationToken cancellationToken);

    Task<OperationResult<AnswerResultModel>>
        ProcessAnswerAsync(AnswerModel answer, CancellationToken cancellationToken);
}