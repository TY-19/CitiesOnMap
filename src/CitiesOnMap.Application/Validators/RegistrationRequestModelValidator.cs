using CitiesOnMap.Application.Models;
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
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.");
    }
}