using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Queries.GetNextCity;

public record GetNextCityRequest(List<string> Previous) : IRequest<City?>;