using FluentValidation;

namespace CitiesOnMap.Application.Features.Games.Requests.GetGame;

public class GetGameRequestValidator : AbstractValidator<GetGameRequest>
{
    public GetGameRequestValidator()
    {
        RuleFor(x => x.GameId)
            .NotEmpty();
    }
}
