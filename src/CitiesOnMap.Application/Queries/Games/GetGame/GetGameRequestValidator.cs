using FluentValidation;

namespace CitiesOnMap.Application.Queries.Games.GetGame;

public class GetGameRequestValidator : AbstractValidator<GetGameRequest>
{
    public GetGameRequestValidator()
    {
        RuleFor(x => x.GameId)
            .NotEmpty();
    }
}