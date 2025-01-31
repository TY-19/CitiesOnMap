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
        IQueryable<City> filteredCities = context.Cities.Include(c => c.Country)
            .Where(c => c.Population > 0
                        && !request.Previous.Contains(c.Name));
        int citiesCount = await filteredCities.CountAsync(cancellationToken);
        int randomSkip = Random.Shared.Next(citiesCount - 1);
        List<City> cities = await filteredCities
            .Skip(randomSkip - 1)
            .Take(1)
            .ToListAsync(cancellationToken);
        return cities.Count == 0 ? null : cities[0];
    }
}