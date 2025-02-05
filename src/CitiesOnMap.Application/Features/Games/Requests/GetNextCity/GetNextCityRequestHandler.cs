using CitiesOnMap.Application.Interfaces.Data;
using CitiesOnMap.Domain.Entities;
using CitiesOnMap.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CitiesOnMap.Application.Features.Games.Requests.GetNextCity;

public class GetNextCityRequestHandler(
    IAppDbContext context)
    : IRequestHandler<GetNextCityRequest, City?>
{
    public async Task<City?> Handle(GetNextCityRequest request, CancellationToken cancellationToken)
    {
        List<City> cities = await context.Cities
            .Include(c => c.Country)
            .Where(c => !request.Game.Previous.Contains(c.Id)
                        && ((c.CapitalType == CapitalType.Primary &&
                             c.Population > request.Game.GameOptions.CapitalsWithPopulationOver)
                            || c.Population > request.Game.GameOptions.CitiesWithPopulationOver))
            .OrderBy(c => Guid.NewGuid())
            .Take(1)
            .ToListAsync(cancellationToken);
        return cities.Count == 0 ? null : cities[0];
    }
}
