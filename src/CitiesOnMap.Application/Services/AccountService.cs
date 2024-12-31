using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CitiesOnMap.Application.Commands.Users.CreateUser;
using CitiesOnMap.Application.Common;
using CitiesOnMap.Application.Interfaces;
using CitiesOnMap.Application.Models;
using CitiesOnMap.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CitiesOnMap.Application.Services;

public class AccountService(
    IMediator mediator,
    IHttpClientFactory factory,
    JwtSecurityTokenHandler jwtSecurityTokenHandler,
    IUserManager userManager,
    IConfiguration configuration)
    : IAccountService
{
    private readonly HttpClient _httpClient = factory.CreateClient();
    private readonly string _jwtSecretKey = GetSecretKey(configuration);

    public async Task<OperationResult<User>> RegisterUserAsync(RegistrationRequestModel model,
        CancellationToken cancellationToken)
    {
        var command = new CreateUserCommand(model.UserName, model.Email, model.Password);
        return await mediator.Send(command, cancellationToken);
    }

    public async Task<OperationResult<TokensModel>> LoginAsync(LoginRequestModel model,
        CancellationToken cancellationToken)
    {
        User? user = null;
        if (model.Email != null)
        {
            user = await userManager.FindByEmailAsync(model.Email);
        }
        else if (model.UserName != null)
        {
            user = await userManager.FindByNameAsync(model.UserName);
        }

        if (user == null)
        {
            return new OperationResult<TokensModel>(false, ResultType.UserNotExist);
        }

        TokensModel tokens = await GenerateTokensForUserAsync(user);

        string refreshHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(tokens.RefreshToken!)));
        await userManager.SetAuthenticationTokenAsync(user, "Local", "Refresh", refreshHash);

        return new OperationResult<TokensModel>(true, tokens);
    }

    public async Task<OperationResult<TokensModel>> LoginExternalUserAsync(string provider, CodeExchangeModel model,
        CancellationToken cancellationToken)
    {
        var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://oauth2.googleapis.com/token")
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", configuration["OAuth:Google:ClientId"] ?? "" },
                { "client_secret", configuration["OAuth:Google:ClientSecret"] ?? "" },
                { "code", model.Code },
                { "redirect_uri", "http://localhost:4200/callback" },
                { "grant_type", "authorization_code" },
                { "code_verifier", model.CodeVerifier }
            })
        };

        HttpResponseMessage response = await _httpClient.SendAsync(tokenRequest, cancellationToken);
        var googleTokens = await response.Content.ReadFromJsonAsync<TokenResult>(cancellationToken);

        using var requestMessage =
            new HttpRequestMessage(HttpMethod.Get, "https://openidconnect.googleapis.com/v1/userinfo");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", googleTokens?.AccessToken);

        HttpResponseMessage infoResponse = await _httpClient.SendAsync(requestMessage, cancellationToken);
        var userInfo = await infoResponse.Content.ReadFromJsonAsync<ExternalUserInfo>(cancellationToken);
        string email = userInfo?.Email
                       ?? throw new Exception("Cannot get user email.");

        User? user = await userManager.FindByLoginAsync("Google", email);
        if (user == null)
        {
            string userName = email[..email.IndexOf('@')];
            var command = new CreateUserCommand(userName, email, null, "Google", email);
            OperationResult<User> result = await mediator.Send(command, cancellationToken);
            user = result.Payload ?? throw new Exception("User not exist");
        }

        TokensModel tokens = await GenerateTokensForUserAsync(user);

        string refreshHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(tokens.RefreshToken!)));
        await userManager.SetAuthenticationTokenAsync(user, "Local", "Refresh", refreshHash);

        return new OperationResult<TokensModel>(true, tokens);
    }

    public async Task<OperationResult<TokensModel>> RefreshTokensAsync(string email, RefreshTokensModel model,
        CancellationToken cancellationToken)
    {
        if (model.AccessToken == null || model.RefreshToken == null)
        {
            return new OperationResult<TokensModel>(false, ResultType.InvalidToken);
        }

        User? user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return new OperationResult<TokensModel>(false, ResultType.UserNotExist);
        }

        string? token = await userManager.GetAuthenticationTokenAsync(user, "Local", "Refresh");
        if (token == null)
        {
            return new OperationResult<TokensModel>(false, ResultType.InvalidToken);
        }

        string[] parts = token.Split("::");
        if (!long.TryParse(parts[0], out long utcTicks)
            || DateTimeOffset.UtcNow.UtcTicks > utcTicks)
        {
            return new OperationResult<TokensModel>(false, ResultType.TokenExpired);
        }

        string tokenHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        if (tokenHash != model.RefreshToken)
        {
            return new OperationResult<TokensModel>(false, ResultType.InvalidToken);
        }

        TokensModel tokens = await GenerateTokensForUserAsync(user);
        string refreshHash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(tokens.RefreshToken!)));
        await userManager.SetAuthenticationTokenAsync(user, "Local", "Refresh", refreshHash);
        return new OperationResult<TokensModel>(true, tokens);
    }

    public async Task<OperationResult> RevokeTokenAsync(string email, RefreshTokensModel model,
        CancellationToken cancellationToken)
    {
        if (model.AccessToken == null || model.RefreshToken == null)
        {
            return new OperationResult(false, ResultType.InvalidToken);
        }

        User? user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return new OperationResult(false, ResultType.UserNotExist);
        }

        string? token = await userManager.GetAuthenticationTokenAsync(user, "Local", "Refresh");
        if (token == null || token != model.RefreshToken)
        {
            return new OperationResult(false, ResultType.InvalidToken);
        }

        await userManager.SetAuthenticationTokenAsync(user, "Local", "Refresh", null);
        return new OperationResult(true);
    }

    private async Task<TokensModel> GenerateTokensForUserAsync(User user)
    {
        if (!int.TryParse(configuration["JwtSettings:ExpirationTimeInMinutes"], out int expirationTime))
        {
            expirationTime = 15;
        }

        DateTimeOffset accessTokenExpiration = DateTimeOffset.UtcNow.AddMinutes(expirationTime);
        string accessToken = await GenerateJwtTokenAsync(user, accessTokenExpiration);

        if (!int.TryParse(configuration["JwtSettings:RefreshTokenValidForMinutes"], out expirationTime))
        {
            expirationTime = 21600;
        }

        DateTimeOffset refreshTokenExpiration = DateTimeOffset.UtcNow.AddDays(expirationTime);
        string refreshToken = GenerateRefreshToken(refreshTokenExpiration);

        return new TokensModel
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiration = accessTokenExpiration,
            RefreshTokenExpiration = refreshTokenExpiration
        };
    }

    private async Task<string> GenerateJwtTokenAsync(User user, DateTimeOffset expiration)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName ?? ""),
            new(ClaimTypes.Email, user.Email ?? ""),
            new(ClaimTypes.NameIdentifier, user.Id)
        };
        IEnumerable<string> roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            Environment.GetEnvironmentVariable("JwtSettings:Issuer")
            ?? configuration["JwtSettings:Issuer"],
            Environment.GetEnvironmentVariable("JwtSettings:Audience")
            ?? configuration["JwtSettings:Audience"],
            claims,
            expires: expiration.DateTime,
            signingCredentials: credentials
        );
        return jwtSecurityTokenHandler.WriteToken(token);
    }

    private static string GenerateRefreshToken(DateTimeOffset expiration)
    {
        var randomBytes = new byte[32];
        var generator = RandomNumberGenerator.Create();
        generator.GetNonZeroBytes(randomBytes);
        string token = Convert.ToBase64String(randomBytes).Replace(":", "_");
        token += "::" + expiration.UtcTicks;
        return token;
    }

    private static string GetSecretKey(IConfiguration configuration)
    {
        string? key = Environment.GetEnvironmentVariable("JwtSettings:SecurityKey");
        if (string.IsNullOrEmpty(key))
        {
            key = configuration["JwtSettings:SecurityKey"];
        }

        return key ?? throw new Exception("JwtSettings: SecurityKey is not configured");
    }
}