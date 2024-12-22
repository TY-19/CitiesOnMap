using CitiesOnMap.Domain.Entities;

namespace CitiesOnMap.Application.Models;

public class AnswerResultModel
{
    public City City { get; set; } = null!;
    public AnswerModel Answer { get; set; } = null!;
    public double Distance { get; set; }
    public int Points { get; set; }
}