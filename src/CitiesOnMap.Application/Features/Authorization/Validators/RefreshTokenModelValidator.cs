using CitiesOnMap.Application.Features.Authorization.Extensions;
using CitiesOnMap.Application.Features.Authorization.Models;
using FluentValidation;

namespace CitiesOnMap.Application.Features.Authorization.Validators;

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
