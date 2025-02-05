using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Features.Games.Requests.GetNextCity;

public record GetNextCityRequest(Game Game) : IRequest<City?>;
