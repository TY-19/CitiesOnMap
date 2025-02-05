using FluentValidation;

namespace CitiesOnMap.Application.Features.Authorization.Requests.GetOAuthProviderConfiguration;

public class GetOAuthProviderConfigurationRequestValidator : AbstractValidator<GetOAuthProviderConfigurationRequest>
{
    public GetOAuthProviderConfigurationRequestValidator()
    {
        RuleFor(x => x.Provider)
            .NotEmpty()
            .WithMessage("Provider cannot be empty.");
    }
}
