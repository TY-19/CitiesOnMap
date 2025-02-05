using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Features.Games.Requests.GetGame;

public record GetGameRequest(string GameId) : IRequest<Game?>;
