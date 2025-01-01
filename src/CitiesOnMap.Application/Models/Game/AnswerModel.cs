namespace CitiesOnMap.Application.Models.Game;

public class AnswerModel
{
    public string GameId { get; set; } = null!;
    public decimal SelectedLatitude { get; set; }
    public decimal SelectedLongitude { get; set; }
}