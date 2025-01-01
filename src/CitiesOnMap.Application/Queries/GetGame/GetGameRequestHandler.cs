using CitiesOnMap.Application.Interfaces.Data;
using CitiesOnMap.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CitiesOnMap.Application.Queries.GetGame;

public class GetGameRequestHandler(
    IAppDbContext context,
    IMemoryCache memoryCache
) : IRequestHandler<GetGameRequest, Game?>
{
    public async Task<Game?> Handle(GetGameRequest request, CancellationToken cancellationToken)
    {
        if (!memoryCache.TryGetValue(request.GameId, out Game? game))
        {
            game = await context.Games.FirstOrDefaultAsync(g => g.Id == request.GameId, cancellationToken);
        }

        return game;
    }
}