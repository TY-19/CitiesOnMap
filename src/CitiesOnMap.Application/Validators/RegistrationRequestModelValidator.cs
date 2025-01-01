using CitiesOnMap.Application.Extensions;
using CitiesOnMap.Application.Models.Authorization;
using FluentValidation;

namespace CitiesOnMap.Application.Validators;

public class RegistrationRequestModelValidator : AbstractValidator<RegistrationRequestModel>
{
    public RegistrationRequestModelValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("UserName is required");
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email should be valid");
        RuleFor(x => x.Password)
            .ApplyPasswordValidationRules();
    }
}