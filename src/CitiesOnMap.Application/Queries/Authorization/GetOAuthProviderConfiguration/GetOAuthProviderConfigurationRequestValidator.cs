using FluentValidation;

namespace CitiesOnMap.Application.Queries.Authorization.GetOAuthProviderConfiguration;

public class GetOAuthProviderConfigurationRequestValidator : AbstractValidator<GetOAuthProviderConfigurationRequest>
{
    public GetOAuthProviderConfigurationRequestValidator()
    {
        RuleFor(x => x.Provider)
            .NotEmpty()
            .WithMessage("Provider cannot be empty.");
    }
}