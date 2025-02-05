namespace CitiesOnMap.Domain.Entities;

public class GameOptions
{
    private string _distanceUnit = "km";
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string GameId { get; set; } = null!;
    public bool ShowCountry { get; set; }
    public bool ShowPopulation { get; set; }
    public int CapitalsWithPopulationOver { get; set; }
    public int CitiesWithPopulationOver { get; set; }

    public string DistanceUnit
    {
        get => _distanceUnit;
        set
        {
            if (value is "km" or "mi")
            {
                _distanceUnit = value;
            }
        }
    }

    public int MaxPointForAnswer { get; set; } = 5000;
    public int ReducePointsPerUnit { get; set; } = 1;
    public bool AllowNegativePoints { get; set; }
}
