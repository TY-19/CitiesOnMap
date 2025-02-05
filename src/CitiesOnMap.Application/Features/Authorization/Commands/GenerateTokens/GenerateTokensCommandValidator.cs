using FluentValidation;

namespace CitiesOnMap.Application.Features.Authorization.Commands.GenerateTokens;

public class GenerateTokensCommandValidator : AbstractValidator<GenerateTokensCommand>
{
    public GenerateTokensCommandValidator()
    {
        RuleFor(x => x.User)
            .NotNull()
            .WithMessage("User cannot be null.");
    }
}
