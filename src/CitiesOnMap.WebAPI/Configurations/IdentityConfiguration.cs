using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CitiesOnMap.WebAPI.Configurations;

public class IdentityConfiguration : IConfigureOptions<IdentityOptions>
{
    public void Configure(IdentityOptions options)
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 8;
    }
}
