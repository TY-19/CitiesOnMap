using CitiesOnMap.Application.Features.Games.Extensions;
using CitiesOnMap.Application.Features.Games.Models;
using CitiesOnMap.Domain.Constants;
using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Features.Games.Commands.CheckAnswer;

public class CheckAnswerCommandHandler : IRequestHandler<CheckAnswerCommand, AnswerResultModel>
{
    public Task<AnswerResultModel> Handle(CheckAnswerCommand command, CancellationToken cancellationToken)
    {
        double distance = CalculateDistance(command.Game.CurrentCity!, command.Answer);
        if (command.Game.GameOptions.DistanceUnit == "mi")
        {
            distance *= AppConstants.KilometersInMile;
        }

        int points = command.Game.GameOptions.MaxPointForAnswer -
                     (int)distance * command.Game.GameOptions.ReducePointsPerUnit;
        if (points < 0 && !command.Game.GameOptions.AllowNegativePoints)
        {
            points = 0;
        }

        var answerResult = new AnswerResultModel
        {
            Answer = command.Answer,
            City = command.Game.CurrentCity!.ToCityDto(),
            Distance = distance,
            Points = points
        };
        command.Game.Previous.Add(command.Game.CurrentCity!.Id);
        command.Game.CurrentCity = null;
        command.Game.Points += points;
        command.Game.LastPlayTime = DateTimeOffset.UtcNow;

        return Task.FromResult(answerResult);
    }

    private static double CalculateDistance(City city, AnswerModel answer)
    {
        double f1 = (double)city.Latitude * Math.PI / 180;
        double l1 = (double)city.Longitude * Math.PI / 180;
        double f2 = (double)answer.SelectedLatitude * Math.PI / 180;
        double l2 = (double)answer.SelectedLongitude * Math.PI / 180;
        double deltaL = l1 * l2 >= 0 ? Math.Abs(l1 - l2) : Math.Abs(l1) + Math.Abs(l2);
        if (deltaL > Math.PI)
        {
            deltaL = Math.PI - deltaL % Math.PI;
        }

        double distance = Math.Atan2(Math.Sqrt(Math.Pow(Math.Cos(f2) * Math.Sin(deltaL), 2)
                                               + Math.Pow(
                                                   Math.Cos(f1) * Math.Sin(f2) -
                                                   Math.Sin(f1) * Math.Cos(f2) * Math.Cos(deltaL), 2)),
            Math.Sin(f1) * Math.Sin(f2) + Math.Cos(f1) * Math.Cos(f2) * Math.Cos(deltaL));
        return distance * AppConstants.EarthRadiusAvg;
    }
}
