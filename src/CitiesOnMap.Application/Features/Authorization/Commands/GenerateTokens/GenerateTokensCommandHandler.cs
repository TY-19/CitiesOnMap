using CitiesOnMap.Application.Features.Authorization.Models;
using CitiesOnMap.Application.Interfaces.Helpers;
using CitiesOnMap.Application.Interfaces.Identity;
using CitiesOnMap.Domain.Constants;
using CitiesOnMap.Domain.Entities;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CitiesOnMap.Application.Features.Authorization.Commands.GenerateTokens;

public class GenerateTokensCommandHandler(
    IUserManager userManager,
    IHashingHelper hasher,
    JwtSecurityTokenHandler jwtSecurityTokenHandler,
    IConfigurationHelper configurationHelper
) : IRequestHandler<GenerateTokensCommand, TokensModel>
{
    private const string AccessTokenExpirationTimeKey = "JwtSettings:ExpirationTimeInMinutes";
    private const string RefreshTokenExpirationTimeKey = "JwtSettings:RefreshTokenValidForMinutes";
    private const string SecurityKeyKey = "JwtSettings:SecurityKey";
    private const string IssuerKey = "JwtSettings:Issuer";
    private const string AudienceKey = "JwtSettings:Audience";

    private readonly string? _accessTokenExpiration =
        configurationHelper.GetConfigurationValue(AccessTokenExpirationTimeKey);

    private readonly string _audience = configurationHelper.GetConfigurationValue(AudienceKey) ?? "";

    private readonly string _issuer = configurationHelper.GetConfigurationValue(IssuerKey) ?? "";

    private readonly string _jwtSecurityKey = configurationHelper.GetConfigurationValue(SecurityKeyKey)
                                              ?? throw new Exception("JWT security key was not configured");

    private readonly string? _refreshTokenExpiration =
        configurationHelper.GetConfigurationValue(RefreshTokenExpirationTimeKey);

    public async Task<TokensModel> Handle(GenerateTokensCommand command, CancellationToken cancellationToken)
    {
        DateTimeOffset accessTokenExpiration = GetExpirationTime(_accessTokenExpiration, 15);
        string accessToken = await GenerateJwtTokenAsync(command.User, accessTokenExpiration);

        DateTimeOffset refreshTokenExpiration = GetExpirationTime(_refreshTokenExpiration, 21600);
        string refreshToken = GenerateRefreshToken(refreshTokenExpiration);

        var tokens = new TokensModel
        {
            UserName = command.User.UserName,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiration = accessTokenExpiration,
            RefreshTokenExpiration = refreshTokenExpiration
        };
        string refreshHash = hasher.GetSha256Hash(tokens.RefreshToken);
        await userManager.SetAuthenticationTokenAsync(command.User, Defaults.DefaultProvider, Defaults.RefreshTokenType,
            refreshHash);
        return tokens;
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

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecurityKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(_issuer, _audience, claims, expires: expiration.DateTime,
            signingCredentials: credentials);
        return jwtSecurityTokenHandler.WriteToken(token);
    }

    private static string GenerateRefreshToken(DateTimeOffset expiration)
    {
        byte[] randomBytes = new byte[32];
        var generator = RandomNumberGenerator.Create();
        generator.GetNonZeroBytes(randomBytes);
        string token = Convert.ToBase64String(randomBytes).Replace(":", "_");
        token += "::" + expiration.UtcTicks;
        return token;
    }

    private static DateTimeOffset GetExpirationTime(string? toParse, int defaultSpanMinutes)
    {
        if (!int.TryParse(toParse, out int minutes))
        {
            minutes = defaultSpanMinutes;
        }

        return DateTimeOffset.UtcNow.AddMinutes(minutes);
    }
}
