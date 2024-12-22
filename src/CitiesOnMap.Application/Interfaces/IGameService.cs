using CitiesOnMap.Application.Common;
using CitiesOnMap.Application.Models;

namespace CitiesOnMap.Application.Interfaces;

public interface IGameService
{
    Task<OperationResult<GameModel>> StartNewGameAsync(string? playerId, CancellationToken cancellationToken);
    Task<OperationResult<GameModel>> GetNextQuestionAsync(string gameId, CancellationToken cancellationToken);

    Task<OperationResult<AnswerResultModel>>
        ProcessAnswerAsync(AnswerModel answer, CancellationToken cancellationToken);
}