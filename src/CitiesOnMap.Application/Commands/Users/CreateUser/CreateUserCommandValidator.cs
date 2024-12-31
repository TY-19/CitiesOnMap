using FluentValidation;

namespace CitiesOnMap.Application.Commands.Users.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("UserName is required.");
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email is in invalid format.");
        RuleFor(x => x.Password)
            .Must((m, p) => m.Provider != null || p != null)
            .WithMessage("Either external provider or password must be specified.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.");
        RuleFor(x => x.Provider)
            .Must((m, p) => m.ProviderKey == null || p != null)
            .WithMessage("Provider must be specified when the provider key is set.");
    }
}