namespace CitiesOnMap.Application.Features.Games.Models;

public class GameModel
{
    public string Id { get; set; } = null!;
    public string PlayerId { get; set; } = null!;
    public int Points { get; set; }
    public string? CurrentCityName { get; set; }
    public int? CityPopulation { get; set; }
    public string? Country { get; set; }
    public DateTimeOffset LastPlayTime { get; set; }
    public GameOptionsModel Options { get; set; } = null!;
}
