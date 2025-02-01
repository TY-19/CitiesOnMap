using CitiesOnMap.Application.Interfaces.Data;
using CitiesOnMap.Domain.Entities;
using CitiesOnMap.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CitiesOnMap.Application.Queries.Games.GetNextCity;

public class GetNextCityRequestHandler(
    IAppDbContext context)
    : IRequestHandler<GetNextCityRequest, City?>
{
    public async Task<City?> Handle(GetNextCityRequest request, CancellationToken cancellationToken)
    {
        IQueryable<City> filteredCities = context.Cities
            .Include(c => c.Country)
            .Where(c => !request.Game.Previous.Contains(c.Id)
                        && ((c.CapitalType == CapitalType.Primary &&
                             c.Population > request.Game.GameOptions.CapitalsWithPopulationOver)
                            || c.Population > request.Game.GameOptions.CitiesWithPopulationOver));
        // int citiesCount = await filteredCities.CountAsync(cancellationToken);
        // int randomSkip = Random.Shared.Next(citiesCount - 1);
        // if (randomSkip < 1)
        // {
        //     randomSkip = 1;
        // }
        List<City> cities = await filteredCities
            .OrderBy(c => Guid.NewGuid())
            .Take(1)
            .ToListAsync(cancellationToken);
        return cities.Count == 0 ? null : cities[0];
    }
}