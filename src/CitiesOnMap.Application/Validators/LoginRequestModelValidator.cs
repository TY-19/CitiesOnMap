using CitiesOnMap.Application.Extensions;
using CitiesOnMap.Application.Models.Authorization;
using FluentValidation;

namespace CitiesOnMap.Application.Validators;

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