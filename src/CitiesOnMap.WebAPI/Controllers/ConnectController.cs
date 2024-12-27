using System.Security.Claims;
using CitiesOnMap.Infrastructure.Identity;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;

namespace CitiesOnMap.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ConnectController(
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
            _ when request.IsPasswordGrantType() => await HandlePasswordFlow(request),
            _ => BadRequest($"Grant type {request.GrantType} is not supported.")
        };
    }
    
    [HttpGet("external-login")]
    public IActionResult ChallengeExternalProvider(string provider, string returnUrl)
    {
        if (string.IsNullOrEmpty(provider))
        {
            return BadRequest("Invalid provider.");
        }

        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(HandleExternalLogin), new { returnUrl }),
            Items =
            {
                ["scheme"] = provider
            }
        };

        return Challenge(properties, provider);
    }

    [HttpGet("external-callback")]
    public async Task<IActionResult> HandleExternalLogin(string? returnUrl = null)
    {
        AuthenticateResult result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded)
        {
            return BadRequest("External authentication failed.");
        }

        ClaimsPrincipal? externalPrincipal = result.Principal;
        string? email = externalPrincipal?.FindFirst(ClaimTypes.Email)?.Value;
        string? name = externalPrincipal?.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            return BadRequest("Email claim is missing.");
        }

        User? user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new User
            {
                UserName = name ?? email,
                Email = email
            };
            IdentityResult identityResult = await userManager.CreateAsync(user);
            if (!identityResult.Succeeded)
            {
                return BadRequest("Failed to create new user.");
            }
        }

        ClaimsPrincipal principal = await signInManager.CreateUserPrincipalAsync(user);
        principal.SetScopes(OpenIddictConstants.Scopes.OpenId, OpenIddictConstants.Scopes.Profile);

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    [HttpGet("userInfo")]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public ActionResult GetUserInfo()
    {
        // Dictionary<string, string> userClaims = User.Claims.Distinct().ToDictionary(c => c.Type, c => c.Value);
        return Ok("Responded");
    }

    private async Task<ActionResult> HandleRefreshTokenFlow()
    {
        AuthenticateResult authenticateResult = await HttpContext
            .AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        ClaimsPrincipal? claimsPrincipal = authenticateResult.Principal;
        return claimsPrincipal == null
            ? Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)
            : SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
    private async Task<ActionResult> HandlePasswordFlow(OpenIddictRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("Username or password cannot be empty");
        }
        
        User? user = await userManager.FindByNameAsync(request.Username);
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        ClaimsPrincipal principal = await signInManager.CreateUserPrincipalAsync(user);
        principal.SetClaim(OpenIddictConstants.Claims.Subject, user.Id);
        principal.SetClaim(OpenIddictConstants.Claims.Email, user.Email);
        principal.SetClaim(OpenIddictConstants.Claims.Role, "User");
        
        principal.SetDestinations(claim => claim.Type switch
        {
            OpenIddictConstants.Claims.Subject => [ OpenIddictConstants.Destinations.IdentityToken, OpenIddictConstants.Destinations.AccessToken],
            OpenIddictConstants.Claims.Role => [OpenIddictConstants.Destinations.IdentityToken],
            OpenIddictConstants.Claims.Email => [OpenIddictConstants.Destinations.AccessToken], 
            _ => [OpenIddictConstants.Destinations.AccessToken]
        });
        
        principal.SetScopes(OpenIddictConstants.Scopes.OpenId, OpenIddictConstants.Scopes.Email,
            OpenIddictConstants.Scopes.Profile, OpenIddictConstants.Scopes.OfflineAccess);
        
        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}
