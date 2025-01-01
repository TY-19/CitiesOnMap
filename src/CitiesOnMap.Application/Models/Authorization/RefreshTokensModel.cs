namespace CitiesOnMap.Application.Models.Authorization;

public class RefreshTokenModel
{
    public string UserName { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}