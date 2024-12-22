using FluentValidation;

namespace CitiesOnMap.Application.Queries.GetGame;

public class GetGameRequestValidator : AbstractValidator<GetGameRequest>
{
    public GetGameRequestValidator()
    {
        RuleFor(x => x.GameId)
            .NotEmpty();
    }
}