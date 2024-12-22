namespace CitiesOnMap.Application.Models;

public class GameModel
{
    public string Id { get; set; } = null!;
    public string PlayerId { get; set; } = null!;
    public int Points { get; set; }
    public string? CurrentCityName { get; set; }
}