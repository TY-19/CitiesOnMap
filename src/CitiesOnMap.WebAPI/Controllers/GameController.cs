using CitiesOnMap.Application.Common;
using CitiesOnMap.Application.Interfaces;
using CitiesOnMap.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace CitiesOnMap.WebAPI.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class GameController(IGameService gameService) : ControllerBase
{
    [HttpPost("start")]
    public async Task<ActionResult<GameModel>> StartGame(string? playerId = null,
        CancellationToken cancellationToken = default)
    {
        OperationResult<GameModel> result = await gameService.StartNewGameAsync(playerId, cancellationToken);
        return HandleOperationResult(result);
    }

    [HttpGet("next-question")]
    public async Task<ActionResult<GameModel>> GetNextQuestion(string gameId, CancellationToken cancellationToken)
    {
        OperationResult<GameModel> result = await gameService.GetNextQuestionAsync(gameId, cancellationToken);
        return HandleOperationResult(result);
    }

    [HttpPost("answer")]
    public async Task<ActionResult<AnswerResultModel>> SendAnswer(AnswerModel answer,
        CancellationToken cancellationToken)
    {
        OperationResult<AnswerResultModel> result = await gameService.ProcessAnswerAsync(answer, cancellationToken);
        return HandleOperationResult(result);
    }

    private ActionResult HandleOperationResult<T>(OperationResult<T> result)
    {
        if (result.Succeeded && result.Payload != null)
        {
            return Ok(result.Payload);
        }

        return BadRequest(result.Details);
    }
}