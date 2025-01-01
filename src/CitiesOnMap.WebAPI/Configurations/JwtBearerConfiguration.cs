using CitiesOnMap.Application.Interfaces.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CitiesOnMap.WebAPI.Configurations;

public class JwtBearerConfiguration(IConfigurationHelper configurationHelper) : IConfigureOptions<JwtBearerOptions>
{
    private const string SecurityKeyKey = "JwtSettings:SecurityKey";
    private const string IssuerKey = "JwtSettings:Issuer";
    private const string AudienceKey = "JwtSettings:Audience";

    public void Configure(JwtBearerOptions options)
    {
        string? key = configurationHelper.GetConfigurationValue(SecurityKeyKey)
                      ?? throw new Exception("JWT security key has not been configured");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            RequireExpirationTime = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configurationHelper.GetConfigurationValue(IssuerKey),
            ValidAudience = configurationHelper.GetConfigurationValue(AudienceKey),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    }
}