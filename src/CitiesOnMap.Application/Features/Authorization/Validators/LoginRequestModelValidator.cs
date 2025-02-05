using CitiesOnMap.Application.Features.Authorization.Extensions;
using CitiesOnMap.Application.Features.Authorization.Models;
using FluentValidation;

namespace CitiesOnMap.Application.Features.Authorization.Validators;

public class LoginRequestModelValidator : AbstractValidator<LoginRequestModel>
{
    public LoginRequestModelValidator()
    {
        RuleFor(x => x)
            .Must(x => x.UserName != null || x.Email != null)
            .WithMessage("At least one of the userName or email must be provided.");
        RuleFor(x => x.Password)
            .ApplyPasswordValidationRules();
    }
}
