namespace CitiesOnMap.Application.Models.Game;

public class GameOptionsModel
{
    public bool? ShowCountry { get; set; }
    public bool? ShowPopulation { get; set; }
    public int? CapitalsWithPopulationOver { get; set; }
    public int? CitiesWithPopulationOver { get; set; }
    public string? DistanceUnit { get; set; }
    public int? MaxPointForAnswer { get; set; }
    public int? ReducePointsPerUnit { get; set; }
    public bool? AllowNegativePoints { get; set; }
}