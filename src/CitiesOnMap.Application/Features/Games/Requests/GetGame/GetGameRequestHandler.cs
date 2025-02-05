using CitiesOnMap.Application.Interfaces.Data;
using CitiesOnMap.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CitiesOnMap.Application.Features.Games.Requests.GetGame;

public class GetGameRequestHandler(
    IAppDbContext context,
    IMemoryCache memoryCache
) : IRequestHandler<GetGameRequest, Game?>
{
    public async Task<Game?> Handle(GetGameRequest request, CancellationToken cancellationToken)
    {
        if (!memoryCache.TryGetValue(request.GameId, out Game? game))
        {
            game = await context.Games
                .Include(g => g.CurrentCity)
                .ThenInclude(c => c!.Country)
                .Include(g => g.GameOptions)
                .FirstOrDefaultAsync(g => g.Id == request.GameId, cancellationToken);
        }

        return game;
    }
}
