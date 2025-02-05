using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Application.Features.Games.Models;

namespace CitiesOnMap.Application.Interfaces.Services;

public interface IGameService
{
    Task<OperationResult<GameModel>> GetGameAsync(string gameId, string playerId,
        CancellationToken cancellationToken);

    Task<OperationResult<GameModel>> StartNewGameAsync(string? playerId,
        GameOptionsModel? optionsModel, CancellationToken cancellationToken);

    Task<OperationResult<GameModel>> GetNextQuestionAsync(string gameId, CancellationToken cancellationToken);

    Task<OperationResult<AnswerResultModel>>
        ProcessAnswerAsync(AnswerModel answer, CancellationToken cancellationToken);

    Task<OperationResult<GameModel>> UpdateGameOptionsAsync(string? playerId, string gameId,
        GameOptionsModel optionsModel, CancellationToken cancellationToken);
}
