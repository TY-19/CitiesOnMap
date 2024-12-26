using System.Security.Claims;
using CitiesOnMap.Infrastructure.Identity;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace CitiesOnMap.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ConnectController(
    IOpenIddictApplicationManager applicationManager,
    UserManager<User> userManager,
    SignInManager<User> signInManager)
    : ControllerBase
{
    [HttpPost("token")]
    [Produces("application/json")]
    public async Task<ActionResult> GetToken()
    {
        OpenIddictRequest? request = HttpContext.GetOpenIddictServerRequest();
        return request switch
        {
            null => BadRequest("Request is null"),
            _ when request.IsRefreshTokenGrantType() => await HandleRefreshTokenFlow(),
            _ when request.IsClientCredentialsGrantType() => await HandleClientCredentialsFlow(request),
            _ when request.IsAuthorizationCodeFlow() => await HandleAuthorizationCodeFlow(),
            _ when request.IsPasswordGrantType() => await HandlePasswordFlow(request),
            _ => BadRequest($"Grant type {request.GrantType} is not supported.")
        };
    }

    [HttpPost("authorize")]
    public async Task<ActionResult> Authorize([FromForm] OpenIddictRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid model state");
        }

        User? user = await userManager.FindByNameAsync(request.Username ?? "");
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password ?? ""))
        {
            return Unauthorized();
        }

        ClaimsPrincipal principal = await signInManager.CreateUserPrincipalAsync(user);
        principal.SetClaim(OpenIddictConstants.Claims.Subject, user.Id);
        principal.SetScopes(OpenIddictConstants.Scopes.OpenId, OpenIddictConstants.Scopes.Profile);
        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private async Task<ActionResult> HandleRefreshTokenFlow()
    {
        ClaimsPrincipal? claimsPrincipal =
            (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme))
            .Principal;
        return SignIn(claimsPrincipal!, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private async Task<ActionResult> HandleClientCredentialsFlow(OpenIddictRequest request)
    {
        object application = await applicationManager.FindByClientIdAsync(request.ClientId ?? "")
                             ?? throw new InvalidOperationException();
        var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType,
            OpenIddictConstants.Claims.Name,
            OpenIddictConstants.Claims.Role);

        identity.SetClaim(OpenIddictConstants.Claims.Subject,
            await applicationManager.GetClientIdAsync(application));
        identity.SetClaim(OpenIddictConstants.Claims.Name,
            await applicationManager.GetDisplayNameAsync(application));
        identity.SetScopes(request.GetScopes());
        identity.SetDestinations(static claim => claim.Type switch
        {
            OpenIddictConstants.Claims.Name when claim.Subject!.HasScope(OpenIddictConstants.Permissions.Scopes
                    .Profile)
                => [OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken],
            _ => [OpenIddictConstants.Destinations.AccessToken]
        });
        return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private async Task<ActionResult> HandleAuthorizationCodeFlow()
    {
        AuthenticateResult user =
            await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        ClaimsPrincipal? principal = user.Principal;
        if (principal == null)
        {
            Console.WriteLine("Principal is null");
            return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        principal.SetClaim("role", "Administrator");

        principal.SetScopes(
            OpenIddictConstants.Scopes.OpenId,
            OpenIddictConstants.Scopes.Profile,
            OpenIddictConstants.Scopes.OfflineAccess
        );
        principal.SetDestinations(claim => claim.Type switch
        {
            OpenIddictConstants.Claims.Name => [OpenIddictConstants.Destinations.IdentityToken],
            "role" => [OpenIddictConstants.Destinations.IdentityToken],
            _ => [OpenIddictConstants.Destinations.AccessToken]
        });
        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private async Task<ActionResult> HandlePasswordFlow(OpenIddictRequest request)
    {
        User? user = await userManager.FindByNameAsync(request.Username ?? "");
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password ?? ""))
        {
            Console.WriteLine("The username/password couple is invalid.");
            return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        ClaimsPrincipal principal = await signInManager.CreateUserPrincipalAsync(user);

        principal.SetClaim(OpenIddictConstants.Claims.Subject, user.Id);

        principal.SetScopes(OpenIddictConstants.Scopes.OpenId, OpenIddictConstants.Scopes.Email,
            OpenIddictConstants.Scopes.Profile, OpenIddictConstants.Scopes.OfflineAccess, "demo_api");
        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}