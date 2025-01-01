using FluentValidation;

namespace CitiesOnMap.Application.Commands.Games.SaveGame;

public class SaveGameCommandValidator : AbstractValidator<SaveGameCommand>
{
    public SaveGameCommandValidator()
    {
        RuleFor(x => x.Game)
            .NotNull()
            .WithMessage("Came cannot be null");
    }
}