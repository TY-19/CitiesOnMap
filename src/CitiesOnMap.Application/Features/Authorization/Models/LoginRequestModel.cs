namespace CitiesOnMap.Application.Features.Authorization.Models;

public class LoginRequestModel
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string Password { get; set; } = null!;
}
