using CitiesOnMap.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CitiesOnMap.Infrastructure.Data.Configurations;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Iso2)
            .HasColumnType("varchar(2)");
        builder.Property(x => x.Iso3)
            .HasColumnType("varchar(3)");
        builder.Property(x => x.Name)
            .HasColumnType("nvarchar(450)");
        builder.HasMany(x => x.Cities)
            .WithOne(x => x.Country)
            .HasForeignKey(x => x.CountryId);
    }
}
