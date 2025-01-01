using CitiesOnMap.Application.Extensions;
using CitiesOnMap.Application.Models.Authorization;
using FluentValidation;

namespace CitiesOnMap.Application.Validators;

public class RefreshTokenModelValidator : AbstractValidator<RefreshTokenModel>
{
    public RefreshTokenModelValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("UserName is required");
        RuleFor(x => x.RefreshToken)
            .ApplyPasswordValidationRules();
    }
}