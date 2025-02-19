using FluentValidation;

namespace CitiesOnMap.Application.Features.Authorization.Requests.ValidateRefreshToken;

public class ValidateRefreshTokenRequestValidator : AbstractValidator<ValidateRefreshTokenRequest>
{
    public ValidateRefreshTokenRequestValidator()
    {
        RuleFor(x => x.User)
            .NotNull()
            .WithMessage("User may not be null.");
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required.");
    }
}
