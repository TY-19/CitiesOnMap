using MediatR;

namespace CitiesOnMap.Application.Features.Games.Commands.PatchGameOptions;

public class PatchGameOptionsCommandHandler : IRequestHandler<PatchGameOptionsCommand>
{
    public Task Handle(PatchGameOptionsCommand command, CancellationToken cancellationToken)
    {
        if (command.UpdatedOptions.ShowCountry.HasValue)
        {
            command.Options.ShowCountry = command.UpdatedOptions.ShowCountry.Value;
        }

        if (command.UpdatedOptions.ShowPopulation.HasValue)
        {
            command.Options.ShowPopulation = command.UpdatedOptions.ShowPopulation.Value;
        }

        if (command.UpdatedOptions.CitiesWithPopulationOver.HasValue)
        {
            command.Options.CitiesWithPopulationOver = command.UpdatedOptions.CitiesWithPopulationOver.Value;
        }

        if (command.UpdatedOptions.CapitalsWithPopulationOver.HasValue)
        {
            command.Options.CapitalsWithPopulationOver = command.UpdatedOptions.CapitalsWithPopulationOver.Value;
        }

        if (!string.IsNullOrEmpty(command.UpdatedOptions.DistanceUnit))
        {
            command.Options.DistanceUnit = command.UpdatedOptions.DistanceUnit;
        }

        if (command.UpdatedOptions.MaxPointForAnswer.HasValue)
        {
            command.Options.MaxPointForAnswer = command.UpdatedOptions.MaxPointForAnswer.Value;
        }

        if (command.UpdatedOptions.ReducePointsPerUnit.HasValue)
        {
            command.Options.ReducePointsPerUnit = command.UpdatedOptions.ReducePointsPerUnit.Value;
        }

        if (command.UpdatedOptions.AllowNegativePoints.HasValue)
        {
            command.Options.AllowNegativePoints = command.UpdatedOptions.AllowNegativePoints.Value;
        }

        return Task.CompletedTask;
    }
}
