using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolicitorFinder.Data.Constants;
using SolicitorFinder.Data.Models;

namespace SolicitorFinder.Data.Configurations;

public sealed class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Title)
            .HasMaxLength(SolicitorConstants.LocationNameMaxLength)
            .IsRequired();

        builder.Property(e => e.Text)
            .HasMaxLength(SolicitorConstants.LocationNameMaxLength)
            .IsRequired();

        builder.Property(l => l.ConfigEntityId)
            .IsRequired(false);

        builder.HasIndex(e => new { e.Title, e.Text })
            .HasDatabaseName("IX_Location_Title_Text");

        builder.HasMany(e => e.SolicitorsLocation)
            .WithOne(e => e.Location)
            .HasForeignKey(e => e.LocationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
