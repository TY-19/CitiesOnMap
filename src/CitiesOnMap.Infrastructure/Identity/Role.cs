using Microsoft.AspNetCore.Identity;

namespace CitiesOnMap.Infrastructure.Identity;

public class Role : IdentityRole
{
    public Role()
    {
    }

    public Role(string roleName) : base(roleName)
    {
    }
}