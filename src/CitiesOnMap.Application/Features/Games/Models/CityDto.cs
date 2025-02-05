using CitiesOnMap.Domain.Enums;

namespace CitiesOnMap.Application.Features.Games.Models;

public class CityDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string NameAscii { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public int CountryId { get; set; }
    public string Country { get; set; } = string.Empty;
    public string AdministrationName { get; set; } = string.Empty;
    public CapitalType CapitalType { get; set; }
    public int Population { get; set; }
}
