using CitiesOnMap.Application.Interfaces.Data;
using CitiesOnMap.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CitiesOnMap.Application.Commands.Games.SaveGame;

public class SaveGameCommandHandler(
    IAppDbContext context,
    IMemoryCache cache
) : IRequestHandler<SaveGameCommand>
{
    public async Task Handle(SaveGameCommand command, CancellationToken cancellationToken)
    {
        cache.Set(command.Game.Id, command.Game);
        Game? game = context.Games
            .Include(g => g.GameOptions)
            .Include(g => g.CurrentCity)
            .FirstOrDefault(g => g.Id == command.Game.Id);
        if (game == null)
        {
            await context.Games.AddAsync(command.Game, cancellationToken);
        }
        else
        {
            game = command.Game;
            context.Games.Update(game);
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}