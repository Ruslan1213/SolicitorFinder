using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolicitorFinder.Data.Constants;
using SolicitorFinder.Data.Models;

namespace SolicitorFinder.Data.Configurations;

public sealed class AreaConfiguration : IEntityTypeConfiguration<AreaEntity>
{
    public void Configure(EntityTypeBuilder<AreaEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .HasMaxLength(SolicitorConstants.NameMaxLength)
            .IsRequired(false);

        builder.Property(e => e.SolicitorAreaExternalId)
            .HasMaxLength(SolicitorConstants.ExternalIdMaxLength)
            .IsRequired(false);

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_Area_Name");

        builder.HasIndex(e => e.SolicitorAreaExternalId)
            .HasDatabaseName("IX_Area_ExternalId");

        builder.HasMany(e => e.SolicitorArea)
           .WithOne(e => e.Area)
           .HasForeignKey(e => e.AreaId)
           .OnDelete(DeleteBehavior.Cascade);
    }
}
