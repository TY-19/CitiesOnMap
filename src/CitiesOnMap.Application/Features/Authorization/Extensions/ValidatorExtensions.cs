using FluentValidation;

namespace CitiesOnMap.Application.Features.Authorization.Extensions;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> ApplyPasswordValidationRules<T>(this IRuleBuilder<T, string> password)
    {
        return password.MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.");
    }
}
