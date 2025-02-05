namespace CitiesOnMap.Application.Features.Authorization.Models;

public class RefreshTokenModel
{
    public string UserName { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}
