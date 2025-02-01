namespace CitiesOnMap.Domain.Entities;

public class Game
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string PlayerId { get; set; } = null!;
    public int Points { get; set; }
    public List<string> Previous { get; set; } = [];
    public City? CurrentCity { get; set; }
    public DateTimeOffset LastPlayTime { get; set; } = DateTimeOffset.UtcNow;
    public GameOptions GameOptions { get; set; } = null!;
}