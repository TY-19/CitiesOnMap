using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Features.Games.Requests.GetGamesForPlayer;

public record GetGamesForPlayerRequest(string PlayerId) : IRequest<List<Game>>;
