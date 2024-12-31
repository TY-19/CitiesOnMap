using Microsoft.AspNetCore.Identity;

namespace CitiesOnMap.Domain.Entities;

public class Role : IdentityRole
{
    public Role()
    {
    }

    public Role(string roleName) : base(roleName)
    {
    }
}