using FluentValidation;

namespace CitiesOnMap.Application.Commands.Authorization.RevokeToken;

public class RevokeTokenCommandValidator : AbstractValidator<RevokeTokenCommand>
{
    public RevokeTokenCommandValidator()
    {
        RuleFor(x => x.User)
            .NotNull()
            .WithMessage("User may not be null.");
        RuleFor(x => x.token)
            .NotEmpty()
            .WithMessage("Token is required.");
    }
}