using CitiesOnMap.Application.Interfaces.Data;
using CitiesOnMap.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CitiesOnMap.Application.Queries.Games.GetNextCity;

public class GetNextCityRequestHandler(
    IAppDbContext context)
    : IRequestHandler<GetNextCityRequest, City?>
{
    public async Task<City?> Handle(GetNextCityRequest request, CancellationToken cancellationToken)
    {
        List<City> cities = await context.Cities
            .Include(c => c.Country)
            .Where(c => c.Population > 0
                        && !request.Previous.Contains(c.Name))
            .Take(100)
            .ToListAsync(cancellationToken);
        return cities.Count == 0 ? null : cities[Random.Shared.Next(cities.Count - 1)];
    }
}