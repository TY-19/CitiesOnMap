using CitiesOnMap.Application.Interfaces.Data;
using CitiesOnMap.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CitiesOnMap.Application.Features.Games.Requests.GetGamesForPlayer;

public class GetGamesForPlayerRequestHandler(IAppDbContext context)
    : IRequestHandler<GetGamesForPlayerRequest, List<Game>>
{
    public async Task<List<Game>> Handle(GetGamesForPlayerRequest request, CancellationToken cancellationToken)
    {
        return await context.Games.Where(g => g.PlayerId == request.PlayerId)
            .Include(g => g.CurrentCity)
            .ThenInclude(c => c!.Country)
            .Include(g => g.GameOptions)
            .ToListAsync(cancellationToken);
    }
}
