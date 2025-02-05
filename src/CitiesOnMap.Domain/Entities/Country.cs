namespace CitiesOnMap.Domain.Entities;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Iso2 { get; set; } = string.Empty;
    public string Iso3 { get; set; } = string.Empty;
    public List<City> Cities { get; set; } = [];
}
