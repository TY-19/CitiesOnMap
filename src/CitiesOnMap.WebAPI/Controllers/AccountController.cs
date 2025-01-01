using CitiesOnMap.Application.Common.Results;
using CitiesOnMap.Application.Interfaces.Services;
using CitiesOnMap.Application.Models.Authorization;
using CitiesOnMap.Application.Models.Authorization.External;
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
        OperationResult<TokensModel> result = await accountService.RegisterUserAsync(model, cancellationToken);
        return HandleOperationResult(result);
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
    [HttpPost("refresh")]
    public async Task<ActionResult<TokensModel>> RefreshTokens(RefreshTokenModel model,
        CancellationToken cancellationToken)
    {
        OperationResult<TokensModel> result = await accountService.RefreshTokensAsync(model, cancellationToken);
        return HandleOperationResult(result);
    }

    [Authorize]
    [HttpPost("revoke")]
    public async Task<ActionResult> RevokeToken(RefreshTokenModel model, CancellationToken cancellationToken)
    {
        OperationResult result = await accountService.RevokeTokenAsync(model, cancellationToken);
        return result.Succeeded ? NoContent() : BadRequest(result.Details.Message);
    }

    [HttpPost("confirm-email")]
    public async Task<ActionResult> ConfirmEmail()
    {
        throw new NotImplementedException();
    }

    [HttpGet("password-reset")]
    public async Task<ActionResult> StartPasswordResetting()
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