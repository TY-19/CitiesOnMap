using CitiesOnMap.Application.Features.Games.Validators;
using FluentValidation;

namespace CitiesOnMap.Application.Features.Games.Commands.CheckAnswer;

public class CheckAnswerCommandValidator : AbstractValidator<CheckAnswerCommand>
{
    public CheckAnswerCommandValidator()
    {
        RuleFor(x => x.Answer)
            .NotNull()
            .SetValidator(new AnswerModelValidator());
        RuleFor(x => x.Game)
            .NotNull();
        RuleFor(x => x.Game.CurrentCity)
            .NotNull()
            .WithMessage("There are no city in the current question to check answer against.");
    }
}
