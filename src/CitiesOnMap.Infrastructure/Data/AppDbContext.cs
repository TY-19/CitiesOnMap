using System.Reflection;
using CitiesOnMap.Application.Interfaces;
using CitiesOnMap.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CitiesOnMap.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<City> Cities => Set<City>();
    public DbSet<Country> Countries => Set<Country>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}