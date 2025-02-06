using FluentValidation;

namespace CitiesOnMap.Application.Features.Games.Requests.GetGamesForPlayer;

public class GetGamesForPlayerRequestValidator : AbstractValidator<GetGamesForPlayerRequest>
{
    public GetGamesForPlayerRequestValidator()
    {
        RuleFor(x => x.PlayerId)
            .NotNull();
    }
}
