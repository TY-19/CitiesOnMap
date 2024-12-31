using System.Security.Claims;
using CitiesOnMap.Application.Common;
using CitiesOnMap.Application.Interfaces;
using CitiesOnMap.Application.Models;
using CitiesOnMap.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitiesOnMap.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IAccountService accountService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<TokensModel>> Registration(RegistrationRequestModel model,
        CancellationToken cancellationToken)
    {
        OperationResult<User> result = await accountService.RegisterUserAsync(model, cancellationToken);
        return result.Succeeded ? NoContent() : BadRequest("Registration has failed.");
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokensModel>> Login(LoginRequestModel model, CancellationToken cancellationToken)
    {
        OperationResult<TokensModel> result = await accountService.LoginAsync(model, cancellationToken);
        return HandleOperationResult(result);
    }

    [HttpPost("login/{provider}")]
    public async Task<ActionResult<TokensModel>> ExternalLogin(string provider, CodeExchangeModel model,
        CancellationToken cancellationToken)
    {
        OperationResult<TokensModel> result =
            await accountService.LoginExternalUserAsync(provider, model, cancellationToken);
        return HandleOperationResult(result);
    }

    [Authorize]
    [HttpPut("refresh")]
    public async Task<ActionResult<TokensModel>> RefreshTokens(RefreshTokensModel model,
        CancellationToken cancellationToken)
    {
        string? email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
        {
            return Unauthorized();
        }

        OperationResult<TokensModel> result = await accountService.RefreshTokensAsync(email, model, cancellationToken);
        return HandleOperationResult(result);
    }

    [Authorize]
    [HttpDelete("revoke")]
    public async Task<ActionResult> RevokeToken(RefreshTokensModel model, CancellationToken cancellationToken)
    {
        string? email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (email == null)
        {
            return Unauthorized();
        }

        OperationResult result = await accountService.RevokeTokenAsync(email, model, cancellationToken);
        return result.Succeeded ? NoContent() : BadRequest(result.Details.Message);
    }

    [HttpPost("confirm-email")]
    public async Task<ActionResult> ConfirmEmail()
    {
        throw new NotImplementedException();
    }

    [HttpGet("password-reset")]
    public async Task<ActionResult> StartPasswordReseting()
    {
        throw new NotImplementedException();
    }

    [HttpPost("password-reset")]
    public async Task<ActionResult> ResetPassword()
    {
        throw new NotImplementedException();
    }

    [Authorize]
    [HttpGet("profile")]
    public ActionResult GetProfile(CancellationToken cancellationToken)
    {
        return Ok("Authorized");
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