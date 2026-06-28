using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolicitorFinder.Data.Constants;
using SolicitorFinder.Data.Models;

namespace SolicitorFinder.Data.Configurations;

public sealed class SolicitorConfiguration : IEntityTypeConfiguration<Solicitor>
{
    public void Configure(EntityTypeBuilder<Solicitor> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        entity.Property(e => e.Name)
            .HasMaxLength(SolicitorConstants.NameMaxLength)
            .IsRequired(false);

        entity.Property(e => e.Phone)
            .HasMaxLength(SolicitorConstants.PhoneMaxLength)
            .IsRequired(false);

        entity.Property(e => e.Address)
            .HasMaxLength(SolicitorConstants.AddressMaxLength)
            .IsRequired(false);

        entity.Property(e => e.Description)
            .HasMaxLength(SolicitorConstants.DescriptionMaxLength)
            .IsRequired(false);

        entity.Property(e => e.Website)
            .HasMaxLength(SolicitorConstants.WebsiteMaxLength)
            .IsRequired(false);

        entity.Property(e => e.RatingStars)
            .HasDefaultValue(0);

        entity.Property(e => e.ReviewCount)
            .HasDefaultValue(0);

        entity.Property(e => e.ScrapedAt)
            .IsRequired();

        entity.HasIndex(e => new { e.RatingStars, e.ReviewCount })
            .HasDatabaseName("IX_Solicitor_Rating_Reviews");

        entity.HasIndex(e => e.Name)
            .HasDatabaseName("IX_Solicitor_Name");

        entity.HasIndex(e => e.Phone)
            .HasDatabaseName("IX_Solicitor_Phone");

        entity.HasMany(e => e.SolicitorLocations)
                .WithOne(e => e.Solicitor)
                .HasForeignKey(e => e.SolicitorId)
                .OnDelete(DeleteBehavior.Cascade);

        entity.HasMany(e => e.SolicitorAreas)
            .WithOne(e => e.Solicitor)
            .HasForeignKey(e => e.SolicitorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
