using CitiesOnMap.Application.Interfaces.Data;
using CitiesOnMap.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CitiesOnMap.Application.Queries.Games.GetGame;

public class GetGameRequestHandler(
    IAppDbContext context,
    IMemoryCache memoryCache,
    ILogger<GetGameRequestHandler> logger
) : IRequestHandler<GetGameRequest, Game?>
{
    public async Task<Game?> Handle(GetGameRequest request, CancellationToken cancellationToken)
    {
        if (!memoryCache.TryGetValue(request.GameId, out Game? game))
        {
            game = await context.Games
                .Include(g => g.CurrentCity)
                .FirstOrDefaultAsync(g => g.Id == request.GameId, cancellationToken);
        }

        logger.LogInformation("Game: {@G}", game);
        return game;
    }
}