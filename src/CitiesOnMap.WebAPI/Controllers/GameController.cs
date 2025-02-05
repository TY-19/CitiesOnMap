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
    [HttpGet("{gameId}")]
    public async Task<ActionResult<GameModel>> GetGame(string gameId, string? playerId,
        CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            playerId ??= User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        OperationResult<GameModel> result = await gameService.GetGameAsync(gameId, playerId ?? "", cancellationToken);
        if (result.Payload == null)
        {
            return NotFound();
        }

        return Ok(result.Payload);
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
    public async Task<ActionResult<GameModel>> ChangeOptions(string? playerId, string gameId, GameOptionsModel options,
        CancellationToken cancellationToken)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            playerId ??= User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        OperationResult<GameModel> result = await gameService.UpdateGameOptionsAsync(
            playerId, gameId, options, cancellationToken);

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
