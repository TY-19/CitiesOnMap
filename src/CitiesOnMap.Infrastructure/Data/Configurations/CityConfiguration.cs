using CitiesOnMap.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CitiesOnMap.Infrastructure.Data.Configurations;

public class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name)
            .HasColumnType("nvarchar(450)");
        builder.Property(x => x.NameAscii)
            .HasColumnType("nvarchar(450)");
        builder.Property(x => x.AdministrationName)
            .HasColumnType("nvarchar(450)");
        builder.Property(x => x.Latitude)
            .HasPrecision(7, 4);
        builder.Property(x => x.Longitude)
            .HasPrecision(7, 4);
    }
}
