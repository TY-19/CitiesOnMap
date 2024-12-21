namespace CitiesOnMap.Domain.Entities;

public class City
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string NameAscii { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public int CountryId { get; set; }
    public Country Country { get; set; } = null!;
    public string AdministrationName { get; set; } = string.Empty;
    public CapitalType CapitalType { get; set; }
    public int Population { get; set; }
}