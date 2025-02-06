using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Application.Features.Games.Models;
using CitiesOnMap.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CitiesOnMap.WebAPI.Controllers;

[Route("/api/[controller]")]
[ApiController]
public class GameController(IGameService gameService) : ControllerBase
{
    [HttpGet("all-current")]
    public async Task<ActionResult<IEnumerable<GameModel>>> GetMyGames(string? playerId = null,
        CancellationToken cancellationToken = default)
    {
        OperationResult<IEnumerable<GameModel>> result = await gameService.GetGamesForPlayerAsync(
            GetUserId(), playerId, cancellationToken);
        return HandleOperationResult(result);
    }

    [HttpGet("{gameId}")]
    public async Task<ActionResult<GameModel>> GetGame(string gameId, string? playerId = null,
        CancellationToken cancellationToken = default)
    {
        OperationResult<GameModel> result =
            await gameService.GetGameAsync(gameId, GetUserId(), playerId, cancellationToken);
        return result.Details.Type == ResultType.GameNotExist
            ? NotFound()
            : HandleOperationResult(result);
    }

    [HttpPost("start")]
    public async Task<ActionResult<GameModel>> StartGame(string? playerId = null,
        GameOptionsModel? options = null,
        CancellationToken cancellationToken = default)
    {
        OperationResult<GameModel> result = await gameService.StartNewGameAsync(playerId, options, cancellationToken);
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

    [HttpPut("options")]
    public async Task<ActionResult<GameModel>> ChangeOptions(string gameId, GameOptionsModel options,
        string? playerId = null, CancellationToken cancellationToken = default)
    {
        OperationResult<GameModel> result = await gameService.UpdateGameOptionsAsync(
            gameId, GetUserId(), playerId, options, cancellationToken);

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

    private string? GetUserId()
    {
        return User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}
