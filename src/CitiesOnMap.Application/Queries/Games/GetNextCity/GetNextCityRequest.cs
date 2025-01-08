using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Queries.Games.GetNextCity;

public record GetNextCityRequest(List<string> Previous) : IRequest<City?>;