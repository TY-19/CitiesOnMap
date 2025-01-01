namespace CitiesOnMap.Application.Models.Authorization;

public class TokensModel
{
    public string? UserName { get; set; }
    public string? AccessToken { get; set; }
    public DateTimeOffset AccessTokenExpiration { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset RefreshTokenExpiration { get; set; }
}