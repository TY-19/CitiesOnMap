using CitiesOnMap.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CitiesOnMap.Infrastructure.Data.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(g => g.Id);
        builder.HasOne(g => g.GameOptions)
            .WithOne()
            .HasForeignKey<GameOptions>(g => g.GameId);
    }
}