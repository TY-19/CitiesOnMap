using FluentValidation;

namespace CitiesOnMap.Application.Queries.Users.GetOAuthProviderConfiguration;

public class GetOAuthProviderConfigurationRequestValidator : AbstractValidator<GetOAuthProviderConfigurationRequest>
{
    public GetOAuthProviderConfigurationRequestValidator()
    {
        RuleFor(x => x.Provider)
            .NotEmpty()
            .WithMessage("Provider cannot be empty.");
    }
}