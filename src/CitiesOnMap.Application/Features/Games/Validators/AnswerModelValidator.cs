using CitiesOnMap.Application.Features.Games.Models;
using FluentValidation;

namespace CitiesOnMap.Application.Features.Games.Validators;

public class AnswerModelValidator : AbstractValidator<AnswerModel>
{
    public AnswerModelValidator()
    {
        RuleFor(x => x.GameId)
            .NotNull();
    }
}
