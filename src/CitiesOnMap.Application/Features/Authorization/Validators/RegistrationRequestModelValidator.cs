using CitiesOnMap.Application.Features.Authorization.Extensions;
using CitiesOnMap.Application.Features.Authorization.Models;
using FluentValidation;

namespace CitiesOnMap.Application.Features.Authorization.Validators;

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
