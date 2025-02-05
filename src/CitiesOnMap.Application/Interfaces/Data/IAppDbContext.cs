using CitiesOnMap.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CitiesOnMap.Application.Interfaces.Data;

public interface IAppDbContext
{
    public DbSet<City> Cities { get; }
    public DbSet<Country> Countries { get; }
    public DbSet<Game> Games { get; }
    public DbSet<GameOptions> GameOptions { get; }
    public DbSet<User> Users { get; }
    public DbSet<Role> Roles { get; }
    public DbSet<UserToken> UserTokens { get; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
