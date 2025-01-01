using FluentValidation;

namespace CitiesOnMap.Application.Queries.Users.GetExternalUserInfo;

public class GetExternalUserInfoRequestValidator : AbstractValidator<GetExternalUserInfoRequest>
{
    public GetExternalUserInfoRequestValidator()
    {
        RuleFor(x => x.Endpoint)
            .NotEmpty()
            .WithMessage("External user info endpoint cannot be null.");
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("External token cannot be null.");
    }
}