using CitiesOnMap.Domain.Entities;
using MediatR;

namespace CitiesOnMap.Application.Queries.GetNextCity;

public class GetNextCityRequest : IRequest<City?>
{
    public List<string> Previous { get; set; } = [];
}