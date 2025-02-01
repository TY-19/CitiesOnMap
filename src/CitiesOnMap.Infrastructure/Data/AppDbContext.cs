using CitiesOnMap.Application.Interfaces.Data;
using CitiesOnMap.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CitiesOnMap.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<User, Role, string, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>(options),
        IAppDbContext
{
    public DbSet<City> Cities => Set<City>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<GameOptions> GameOptions => Set<GameOptions>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}