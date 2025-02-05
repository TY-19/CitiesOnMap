namespace CitiesOnMap.Application.Features.Games.Models;

public class AnswerResultModel
{
    public CityDto City { get; set; } = null!;
    public AnswerModel Answer { get; set; } = null!;
    public double Distance { get; set; }
    public int Points { get; set; }
}
