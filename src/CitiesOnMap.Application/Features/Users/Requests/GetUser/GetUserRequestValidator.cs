using FluentValidation;

namespace CitiesOnMap.Application.Features.Users.Requests.GetUser;

public class GetUserRequestValidator : AbstractValidator<GetUserRequest>
{
    public GetUserRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.UserId is not null || x.UserName is not null || x.Email is not null
                       || x is { Provider: not null, ProviderKey: not null })
            .WithMessage(
                "Either at least one of UserId, UserName, Email or both of Provider and ProviderKey must be not null");
    }
}
