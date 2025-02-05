using FluentValidation;

namespace CitiesOnMap.Application.Features.Games.Commands.SaveGame;

public class SaveGameCommandValidator : AbstractValidator<SaveGameCommand>
{
    public SaveGameCommandValidator()
    {
        RuleFor(x => x.Game)
            .NotNull()
            .WithMessage("Came cannot be null");
    }
}
