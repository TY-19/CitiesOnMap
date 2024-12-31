namespace CitiesOnMap.Application.Models;

public class TokensModel
{
    public string? AccessToken { get; set; }
    public DateTimeOffset AccessTokenExpiration { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset RefreshTokenExpiration { get; set; }
}