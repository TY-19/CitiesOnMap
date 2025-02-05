using CitiesOnMap.Application.Features.Games.Validators;
using FluentValidation;

namespace CitiesOnMap.Application.Features.Games.Commands.PatchGameOptions;

public class PatchGameOptionsCommandValidator : AbstractValidator<PatchGameOptionsCommand>
{
    public PatchGameOptionsCommandValidator()
    {
        RuleFor(x => x.Options)
            .NotNull();
        RuleFor(x => x.UpdatedOptions)
            .SetValidator(new GameOptionsModelValidator());
    }
}
