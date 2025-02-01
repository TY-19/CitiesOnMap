namespace CitiesOnMap.Application.Models.Game;

public class AnswerResultModel
{
    public CityDto City { get; set; } = null!;
    public AnswerModel Answer { get; set; } = null!;
    public double Distance { get; set; }
    public int Points { get; set; }
}