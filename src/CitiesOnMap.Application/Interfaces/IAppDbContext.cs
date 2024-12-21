using CitiesOnMap.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CitiesOnMap.Application.Interfaces;

public interface IAppDbContext
{
    public DbSet<City> Cities { get; }
    public DbSet<Country> Countries { get; }
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}