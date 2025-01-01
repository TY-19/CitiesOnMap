using FluentValidation;

namespace CitiesOnMap.Application.Commands.Authorization.GenerateTokens;

public class GenerateTokensCommandValidator : AbstractValidator<GenerateTokensCommand>
{
    public GenerateTokensCommandValidator()
    {
        RuleFor(x => x.User)
            .NotNull()
            .WithMessage("User cannot be null.");
    }
}