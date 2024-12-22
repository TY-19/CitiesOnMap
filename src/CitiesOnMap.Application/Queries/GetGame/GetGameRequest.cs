using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Queries.GetGame;

public class GetGameRequest(string gameId) : IRequest<Game?>
{
    public string GameId { get; } = gameId;
}