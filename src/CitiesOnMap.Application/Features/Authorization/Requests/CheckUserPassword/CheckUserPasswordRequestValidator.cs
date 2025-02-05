using FluentValidation;

namespace CitiesOnMap.Application.Features.Authorization.Requests.CheckUserPassword;

public class CheckUserPasswordRequestValidator : AbstractValidator<CheckUserPasswordRequest>
{
    public CheckUserPasswordRequestValidator()
    {
        RuleFor(x => x.User)
            .NotNull()
            .WithMessage("User cannot be null");
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}
