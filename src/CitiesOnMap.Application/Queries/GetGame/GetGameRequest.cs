using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Queries.GetGame;

public record GetGameRequest(string GameId) : IRequest<Game?>;