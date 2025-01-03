using CitiesOnMap.Application.Interfaces.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CitiesOnMap.WebAPI.Configurations;

public class JwtBearerConfiguration(IConfigurationHelper configurationHelper) : IConfigureNamedOptions<JwtBearerOptions>
{
    private const string SecurityKeyKey = "JwtSettings:SecurityKey";
    private const string IssuerKey = "JwtSettings:Issuer";
    private const string AudienceKey = "JwtSettings:Audience";

    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme)
        {
            return;
        }

        string key = configurationHelper.GetConfigurationValue(SecurityKeyKey)
                     ?? throw new Exception("JWT security key has not been configured");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            RequireExpirationTime = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configurationHelper.GetConfigurationValue(IssuerKey),
            ValidAudience = configurationHelper.GetConfigurationValue(AudienceKey),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    }

    public void Configure(JwtBearerOptions options)
    {
    }
}