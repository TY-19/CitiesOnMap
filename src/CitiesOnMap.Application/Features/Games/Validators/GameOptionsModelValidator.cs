using CitiesOnMap.Application.Features.Games.Models;
using FluentValidation;

namespace CitiesOnMap.Application.Features.Games.Validators;

public class GameOptionsModelValidator : AbstractValidator<GameOptionsModel>
{
    public GameOptionsModelValidator()
    {
        RuleFor(x => x.CapitalsWithPopulationOver)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Population cannot be a negative number");
        RuleFor(x => x.CitiesWithPopulationOver)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Population cannot be a negative number");
        RuleFor(x => x.DistanceUnit)
            .Must(d => d is "km" or "mi")
            .WithMessage("Distance unit must be 'km' for kilometers or 'mi' for miles");
        RuleFor(x => x.MaxPointForAnswer)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Max points cannot be a negative number");
        RuleFor(x => x.ReducePointsPerUnit)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Reduce points per unit must be 0 or higher");
    }
}
