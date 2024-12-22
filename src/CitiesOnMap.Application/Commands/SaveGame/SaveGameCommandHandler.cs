using CitiesOnMap.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace CitiesOnMap.Application.Commands.SaveGame;

public class SaveGameCommandHandler(
    IAppDbContext context,
    IMemoryCache cache
) : IRequestHandler<SaveGameCommand>
{
    public async Task Handle(SaveGameCommand command, CancellationToken cancellationToken)
    {
        cache.Set(command.Game.Id, command.Game);
        await context.Games.AddAsync(command.Game, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}